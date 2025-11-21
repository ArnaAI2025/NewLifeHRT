using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class PharmacyConfigurationControllerTests
    {
        private readonly Mock<IPharmacyConfigurationService> _serviceMock = new();

        [Fact]
        public async Task CreatePharmacyConfiguration_Should_ReturnBadRequest_When_RequestNull()
        {
            var controller = new PharmacyConfigurationController(_serviceMock.Object);

            var result = await controller.CreatePharmacyConfiguration(null!);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreatePharmacyConfiguration_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = new PharmacyConfigurationController(_serviceMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            var result = await controller.CreatePharmacyConfiguration(new PharmacyConfigurationRequestDto { ConfigData = new[] { new PharmacyConfigurationDataRequestDto { KeyId = 1, Value = "v" } } });

            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task GetById_Should_ReturnNotFound_When_ServiceReturnsNull()
        {
            var controller = new PharmacyConfigurationController(_serviceMock.Object);

            var result = await controller.GetById(Guid.NewGuid());

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
