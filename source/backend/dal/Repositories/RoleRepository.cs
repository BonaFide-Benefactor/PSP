using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pims.Core.Extensions;
using Pims.Core.Http.Configuration;
using Pims.Dal.Entities;
using Pims.Dal.Entities.Models;
using Pims.Dal.Helpers.Extensions;
using Pims.Dal.Security;

namespace Pims.Dal.Repositories
{
    /// <summary>
    /// RoleRepository class, provides a service layer to administrate roles within the datasource.
    /// </summary>
    public class RoleRepository : BaseRepository<PimsRole>, IRoleRepository
    {
        #region Variables
        private readonly IOptionsMonitor<AuthClientOptions> _keycloakOptions;
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of a RoleRepository, and initializes it with the specified arguments.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="user"></param>
        /// <param name="logger"></param>
        public RoleRepository(PimsContext dbContext, ClaimsPrincipal user, IOptionsMonitor<AuthClientOptions> options, ILogger<RoleRepository> logger)
            : base(dbContext, user, logger)
        {
            this._keycloakOptions = options;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Get a page of roles from the datasource.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="quantity"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Paged<PimsRole> Get(int page, int quantity, string name = null)
        {
            this.User.ThrowIfNotAuthorizedOrServiceAccount(Permissions.AdminRoles, _keycloakOptions);

            var query = this.Context.PimsRoles.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(r => EF.Functions.Like(r.Name, $"%{name}%"));
            }

            var roles = query.Skip((page - 1) * quantity).Take(quantity);
            return new Paged<PimsRole>(roles.ToArray(), page, quantity, query.Count());
        }

        /// <summary>
        /// Get the role with the specified 'id'.
        /// </summary>
        /// <param name="key"></param>
        /// <exception type="KeyNotFoundException">Entity does not exist in the datasource.</exception>
        /// <returns></returns>
        public PimsRole Get(Guid key)
        {
            return this.Context.PimsRoles
                .Include(r => r.PimsRoleClaims).ThenInclude(c => c.Claim)
                .AsNoTracking()
                .FirstOrDefault(u => u.RoleUid == key) ?? throw new KeyNotFoundException();
        }

        /// <summary>
        /// Get the role with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <exception type="KeyNotFoundException">Entity does not exist in the datasource.</exception>
        /// <returns></returns>
        public PimsRole GetByName(string name)
        {
            return this.Context.PimsRoles
                .Include(r => r.PimsRoleClaims).ThenInclude(c => c.Claim)
                .AsNoTracking()
                .FirstOrDefault(r => r.Name == name) ?? throw new KeyNotFoundException();
        }

        /// <summary>
        /// Get the user with the specified keycloak group 'key'.
        /// </summary>
        /// <param name="key"></param>
        /// <exception type="KeyNotFoundException">Entity does not exist in the datasource.</exception>
        /// <returns></returns>
        public PimsRole GetByKeycloakId(Guid key)
        {
            this.User.ThrowIfNotAuthorized(Permissions.AdminRoles);

            return this.Context.PimsRoles
                .Include(r => r.PimsRoleClaims).ThenInclude(c => c.Claim)
                .AsNoTracking()
                .FirstOrDefault(u => u.KeycloakGroupId == key) ?? throw new KeyNotFoundException();
        }

        /// <summary>
        /// Add the specified role to the datasource.
        /// </summary>
        /// <param name="add"></param>
        /// <returns></returns>
        public PimsRole Add(PimsRole add)
        {
            AddWithoutSave(add);
            this.Context.CommitTransaction();
            return add;
        }

        /// <summary>
        /// Add the specified role to the datasource.
        /// </summary>
        /// <param name="add"></param>
        public void AddWithoutSave(PimsRole add)
        {
            add.ThrowIfNull(nameof(add));
            this.User.ThrowIfNotAuthorized(Permissions.AdminRoles);

            this.Context.PimsRoles.Add(add);
        }

        /// <summary>
        /// Updates the specified role in the datasource.
        /// </summary>
        /// <param name="update"></param>
        /// <exception type="KeyNotFoundException">Entity does not exist in the datasource.</exception>
        /// <returns></returns>
        public PimsRole Update(PimsRole update)
        {
            var role = UpdateWithoutSave(update);
            this.Context.CommitTransaction();
            return role;
        }

        /// <summary>
        /// Updates the specified role in the datasource.
        /// </summary>
        /// <param name="update"></param>
        /// <exception type="KeyNotFoundException">Entity does not exist in the datasource.</exception>
        /// <returns></returns>
        public PimsRole UpdateWithoutSave(PimsRole update)
        {
            update.ThrowIfNull(nameof(update));
            this.User.ThrowIfNotAuthorizedOrServiceAccount(Permissions.AdminRoles, _keycloakOptions);

            var role = this.Context.PimsRoles.Find(update.RoleId) ?? throw new KeyNotFoundException();

            this.Context.Entry(role).CurrentValues.SetValues(update);
            this.Context.SetOriginalConcurrencyControlNumber(role);
            return role;
        }

        /// <summary>
        /// Remove the specified role from the datasource.
        /// </summary>
        /// <param name="delete"></param>
        /// <exception type="KeyNotFoundException">Entity does not exist in the datasource.</exception>
        public void Delete(PimsRole delete)
        {
            delete.ThrowIfNull(nameof(delete));
            this.User.ThrowIfNotAuthorized(Permissions.AdminRoles);

            var role = this.Context.PimsRoles.Find(delete.RoleId) ?? throw new KeyNotFoundException();

            role.ConcurrencyControlNumber = delete.ConcurrencyControlNumber;
            this.Context.SetOriginalConcurrencyControlNumber(role);
            this.Context.PimsRoles.Remove(role);
            this.Context.CommitTransaction();
        }

        /// <summary>
        /// Remove the roles from the datasource, excluding those listed.
        /// </summary>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public int RemoveAll(Guid[] exclude)
        {
            this.User.ThrowIfNotAuthorized(Permissions.AdminRoles);
            var roles = this.Context.PimsRoles.Include(r => r.PimsRoleClaims).Include(r => r.PimsUserRoles).Where(r => !exclude.Contains(r.RoleUid));
            roles.ForEach(r =>
            {
                r.PimsRoleClaims.Clear();
                r.PimsUserRoles.Clear();
            });

            this.Context.PimsRoles.RemoveRange(roles);
            var result = this.Context.CommitTransaction();
            return result;
        }
        #endregion
    }
}
