using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pims.Api.Areas.Lease.Controllers;
using Pims.Core.Test;
using Pims.Dal;
using Pims.Dal.Repositories;
using Pims.Dal.Security;
using Xunit;
using Model = Pims.Api.Areas.Lease.Models.Lease;

namespace Pims.Api.Test.Controllers.Lease
{
    [Trait("category", "unit")]
    [Trait("category", "api")]
    [Trait("group", "lease")]
    [ExcludeFromCodeCoverage]
    public class LeaseTenantControllerTest
    {
        #region Tests
        #region GetProperties
        /// <summary>
        /// Make a successful request.
        /// </summary>
        [Fact]
        public void UpdateLeaseTenants_Success()
        {
            // Arrange
            var helper = new TestHelper();
            var controller = helper.CreateController<LeaseTenantController>(Permissions.LeaseEdit);

            var lease = EntityHelper.CreateLease(1);
            var person = EntityHelper.CreatePerson(1, "tester", "chester");
            var organization = EntityHelper.CreateOrganization(1, "tester org");
            lease.PimsLeaseTenants.Add(new Dal.Entities.PimsLeaseTenant() { LeaseId = lease.LeaseId, PersonId = person.PersonId });
            lease.PimsLeaseTenants.Add(new Dal.Entities.PimsLeaseTenant() { LeaseId = lease.LeaseId, OrganizationId = organization.OrganizationId });

            var repository = helper.GetService<Mock<ILeaseRepository>>();
            var mapper = helper.GetService<IMapper>();

            repository.Setup(m => m.UpdateLeaseTenants(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<ICollection<Pims.Dal.Entities.PimsLeaseTenant>>())).Returns(lease);

            // Act
            var result = controller.UpdateTenants(lease.LeaseId, mapper.Map<Model.LeaseModel>(lease));

            // Assert
            var actionResult = Assert.IsType<JsonResult>(result);
            var actualResult = Assert.IsType<Model.LeaseModel>(actionResult.Value);
            var expectedResult = mapper.Map<Model.LeaseModel>(lease);
            expectedResult.Should().BeEquivalentTo(actualResult);
            repository.Verify(m => m.UpdateLeaseTenants(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<ICollection<Pims.Dal.Entities.PimsLeaseTenant>>()), Times.Once());
        }
        #endregion
        #endregion
    }
}
