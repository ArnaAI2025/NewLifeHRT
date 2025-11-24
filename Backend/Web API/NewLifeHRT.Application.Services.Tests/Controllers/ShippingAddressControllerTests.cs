using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class ShippingAddressControllerTests
    {
        private static ShippingAddressController CreateController(
            Mock<IShippingAddressService> shippingAddressService,
            Mock<ICountryService> countryService,
            Mock<IStateService> stateService,
            int? userId = null)
        {
            var controller = new ShippingAddressController(
                shippingAddressService.Object,
                countryService.Object,
                stateService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = userId.HasValue
                            ? new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()) }))
                            : new ClaimsPrincipal(new ClaimsIdentity())
                    }
                }
            };

            return controller;
        }

        [Fact]
        public async Task Create_Should_ReturnUnauthorized_When_UserMissing()
        {
            // Arrange
            var controller = CreateController(new Mock<IShippingAddressService>(), new Mock<ICountryService>(), new Mock<IStateService>());

            // Act
            var result = await controller.Create(new ShippingAddressRequestDto());

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task BulkToggleActiveStatus_Should_ReturnUnauthorized_When_UserMissing()
        {
            // Arrange
            var controller = CreateController(new Mock<IShippingAddressService>(), new Mock<ICountryService>(), new Mock<IStateService>());

            // Act
            var result = await controller.BulkToggleActiveStatus(new BulkOperationRequestDto<Guid>());

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task BulkDelete_Should_ReturnOk_WithServiceResponse()
        {
            // Arrange
            var response = new BulkOperationResponseDto { Message = "done" };
            var shippingServiceMock = new Mock<IShippingAddressService>();
            shippingServiceMock.Setup(s => s.BulkDeleteAsync(It.IsAny<IList<Guid>>())).ReturnsAsync(response);

            var controller = CreateController(shippingServiceMock, new Mock<ICountryService>(), new Mock<IStateService>());

            // Act
            var result = await controller.BulkDelete(new BulkOperationRequestDto<Guid> { Ids = new List<Guid> { Guid.NewGuid() } });

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(response);
        }
    }
}