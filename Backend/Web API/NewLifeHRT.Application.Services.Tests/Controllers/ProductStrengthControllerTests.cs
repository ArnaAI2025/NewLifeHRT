using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using System;
using System.Security.Claims;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class ProductStrengthControllerTests
    {
        private static ProductStrengthController CreateController(Mock<IProductStrengthService>? service = null, int? userId = null)
        {
            service ??= new Mock<IProductStrengthService>();

            var controller = new ProductStrengthController(service.Object)
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
        public async Task GetAllByProductId_Should_ReturnBadRequest_When_ProductIdEmpty()
        {
            var controller = CreateController();

            var result = await controller.GetAllByProductId(Guid.Empty);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateProductStrength_Should_ReturnBadRequest_When_ModelInvalid()
        {
            var controller = CreateController(userId: 2);
            controller.ModelState.AddModelError("Name", "Required");

            var result = await controller.CreateProductStrength(new ProductStrengthCreateRequestDto());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteProductStrength_Should_ReturnOk_WithId()
        {
            var service = new Mock<IProductStrengthService>();
            var id = Guid.NewGuid();
            service.Setup(s => s.DeleteAsync(id)).ReturnsAsync(id);

            var controller = CreateController(service);

            var result = await controller.DeleteProductStrength(id) as OkObjectResult;

            result.Should().NotBeNull();
            result!.Value.Should().Be(id);
        }
    }
}