using System;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pims.Api.Policies;
using Pims.Dal.Entities;
using Pims.Dal.Repositories;
using Pims.Dal.Security;
using Swashbuckle.AspNetCore.Annotations;
using Entity = Pims.Dal.Entities;
using Model = Pims.Api.Models.Concepts;

namespace Pims.Api.Areas.Admin.Controllers
{
    /// <summary>
    /// ClaimController class, provides endpoints for managing claims.
    /// </summary>
    [HasPermission(Permissions.SystemAdmin)]
    [Authorize]
    [ApiController]
    [Area("admin")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[area]/claims")]
    [Route("[area]/claims")]
    public class ClaimController : ControllerBase
    {
        #region Variables
        private readonly IClaimRepository _claimRepository;
        private readonly IMapper _mapper;
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of a ClaimController class, initializes it with the specified arguments.
        /// </summary>
        /// <param name="claimRepository"></param>
        /// <param name="mapper"></param>
        public ClaimController(IClaimRepository claimRepository, IMapper mapper)
        {
            _claimRepository = claimRepository;
            _mapper = mapper;
        }
        #endregion

        #region Endpoints

        /// <summary>
        /// GET - Returns a paged array of claims from the datasource.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="quantity"></param>
        /// <param name="name"></param>
        /// <returns>Paged object with an array of claims.</returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Api.Models.PageModel<Model.ClaimModel>), 200)]
        [ProducesResponseType(typeof(Api.Models.ErrorResponseModel), 400)]
        [SwaggerOperation(Tags = new[] { "admin-claim" })]
        public IActionResult GetClaims(int page = 1, int quantity = 10, string name = null)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (quantity < 1)
            {
                quantity = 1;
            }

            if (quantity > 50)
            {
                quantity = 50;
            }

            var paged = _claimRepository.Get(page, quantity, name);
            var result = _mapper.Map<Api.Models.PageModel<Model.ClaimModel>>(paged);
            return new JsonResult(result);
        }

        /// <summary>
        /// GET - Returns a claim for the specified 'key' from the datasource.
        /// </summary>
        /// <param name="key">The unique 'key' for the claim to return.</param>
        /// <returns>The claim requested.</returns>
        [HttpGet("{key}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Model.ClaimModel), 200)]
        [ProducesResponseType(typeof(Api.Models.ErrorResponseModel), 400)]
        [SwaggerOperation(Tags = new[] { "admin-claim" })]
        public IActionResult GetClaim(Guid key)
        {
            var entity = _claimRepository.Get(key);
            var claim = _mapper.Map<Model.ClaimModel>(entity);
            return new JsonResult(claim);
        }

        /// <summary>
        /// POST - Add a new claim to the datasource.
        /// </summary>
        /// <param name="model">The claim model.</param>
        /// <returns>The claim added.</returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Model.ClaimModel), 201)]
        [ProducesResponseType(typeof(Api.Models.ErrorResponseModel), 400)]
        [SwaggerOperation(Tags = new[] { "admin-claim" })]
        public IActionResult AddClaim([FromBody] Model.ClaimModel model)
        {
            var entity = _mapper.Map<Entity.PimsClaim>(model); // TODO: Return bad request.
            _claimRepository.Add(entity);
            var claim = _mapper.Map<Model.ClaimModel>(entity);

            return CreatedAtAction(nameof(GetClaim), new { id = claim.Id }, claim);
        }

        /// <summary>
        /// PUT - Update the claim in the datasource.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="model">The claim model.</param>
        /// <returns>The claim updated.</returns>
        [HttpPut("{key}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Model.ClaimModel), 200)]
        [ProducesResponseType(typeof(Api.Models.ErrorResponseModel), 400)]
        [SwaggerOperation(Tags = new[] { "admin-claim" })]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameter 'key' is required for route.")]
        public IActionResult UpdateClaim(Guid key, [FromBody] Model.ClaimModel model)
        {
            var entity = _mapper.Map<PimsClaim>(model);
            _claimRepository.Update(entity);

            var claim = _mapper.Map<Model.ClaimModel>(entity);
            return new JsonResult(claim);
        }

        /// <summary>
        /// DELETE - Delete the claim from the datasource.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="model">The claim model.</param>
        /// <returns>The claim who was deleted.</returns>
        [HttpDelete("{key}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Model.ClaimModel), 200)]
        [ProducesResponseType(typeof(Api.Models.ErrorResponseModel), 400)]
        [SwaggerOperation(Tags = new[] { "admin-claim" })]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameter 'key' is required for route.")]
        public IActionResult DeleteClaim(Guid key, [FromBody] Model.ClaimModel model)
        {
            var entity = _mapper.Map<PimsClaim>(model);
            _claimRepository.Delete(entity);

            return new JsonResult(model);
        }
        #endregion
    }
}
