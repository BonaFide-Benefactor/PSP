using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Pims.Api.Controllers;
using Pims.Core.Test;
using Pims.Dal;
using Pims.Dal.Repositories;
using Pims.Dal.Security;
using Xunit;
using Entity = Pims.Dal.Entities;
using Model = Pims.Api.Models.Tenant;

namespace Pims.Api.Test.Controllers
{
    [Trait("category", "unit")]
    [Trait("category", "api")]
    [Trait("group", "tenants")]
    [ExcludeFromCodeCoverage]
    public class TenantControllerTest
    {
        #region Data
        #endregion

        #region Tests
        #region Settings
        [Fact]
        public void Settings_200Response()
        {
            // Arrange
            var helper = new TestHelper();
            var user = PrincipalHelper.CreateForPermission();
            var mockOptions = new Mock<IOptionsMonitor<PimsOptions>>();
            var options = new PimsOptions()
            {
                Tenant = "TEST",
            };
            mockOptions.Setup(m => m.CurrentValue).Returns(options);
            var controller = helper.CreateController<TenantController>(user, mockOptions.Object);

            var repository = helper.GetService<Mock<ITenantRepository>>();
            var mapper = helper.GetService<IMapper>();
            var tenant = EntityHelper.CreateTenant(1, "TEST");
            repository.Setup(m => m.GetTenant(tenant.Code)).Returns(tenant);

            // Act
            var result = controller.Settings();

            // Assert
            var actionResult = Assert.IsType<JsonResult>(result);
            var actualResult = Assert.IsType<Model.TenantModel>(actionResult.Value);
            Assert.Null(actionResult.StatusCode);
            mapper.Map<Model.TenantModel>(tenant).Should().BeEquivalentTo(actualResult);
            repository.Verify(m => m.GetTenant(tenant.Code), Times.Once());
        }

        [Fact]
        public void Settings_NoTenantFound_204Response()
        {
            // Arrange
            var helper = new TestHelper();
            var user = PrincipalHelper.CreateForPermission();
            var mockOptions = new Mock<IOptionsMonitor<PimsOptions>>();
            var options = new PimsOptions()
            {
                Tenant = "TEST",
            };
            mockOptions.Setup(m => m.CurrentValue).Returns(options);
            var controller = helper.CreateController<TenantController>(user, mockOptions.Object);

            var repository = helper.GetService<Mock<ITenantRepository>>();
            var tenant = EntityHelper.CreateTenant(1, "TEST");
            repository.Setup(m => m.GetTenant(tenant.Code)).Returns<Entity.PimsTenant>(null);

            // Act
            var result = controller.Settings();

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
            actionResult.StatusCode.Should().Be(204);
            repository.Verify(m => m.GetTenant(tenant.Code), Times.Once());
        }

        [Fact]
        public void Settings_TenantOptions_204Response()
        {
            // Arrange
            var helper = new TestHelper();
            var user = PrincipalHelper.CreateForPermission();
            var mockOptions = new Mock<IOptionsMonitor<PimsOptions>>();
            var options = new PimsOptions();
            mockOptions.Setup(m => m.CurrentValue).Returns(options);
            var controller = helper.CreateController<TenantController>(user, mockOptions.Object);

            var repository = helper.GetService<Mock<ITenantRepository>>();
            repository.Setup(m => m.GetTenant(null)).Returns<Entity.PimsTenant>(null);

            // Act
            var result = controller.Settings();

            // Assert
            var actionResult = Assert.IsType<NoContentResult>(result);
            actionResult.StatusCode.Should().Be(204);
            repository.Verify(m => m.GetTenant(null), Times.Once());
        }
        #endregion

        #region UpdateTenant
        [Fact]
        public void UpdateTenant_200Response()
        {
            // Arrange
            var helper = new TestHelper();
            var controller = helper.CreateController<TenantController>(Permissions.SystemAdmin);

            var repository = helper.GetService<Mock<ITenantRepository>>();
            var mapper = helper.GetService<IMapper>();
            var tenant = EntityHelper.CreateTenant(1, "TEST");
            repository.Setup(m => m.UpdateTenant(It.IsAny<Entity.PimsTenant>())).Returns(tenant);

            var model = mapper.Map<Model.TenantModel>(tenant);

            // Act
            var result = controller.UpdateTenant(tenant.Code, model);

            // Assert
            var actionResult = Assert.IsType<JsonResult>(result);
            var actualResult = Assert.IsType<Model.TenantModel>(actionResult.Value);
            Assert.Null(actionResult.StatusCode);
            model.Should().BeEquivalentTo(actualResult);
            repository.Verify(m => m.UpdateTenant(It.IsAny<Entity.PimsTenant>()), Times.Once());
        }
        #endregion
        #endregion
    }
}
