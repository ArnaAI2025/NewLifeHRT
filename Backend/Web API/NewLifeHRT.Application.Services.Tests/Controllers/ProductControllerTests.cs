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
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class ProductControllerTests
    {
        private static ProductController CreateController(
            Mock<IProductService>? productService = null,
            Mock<IProductTypeService>? productTypeService = null,
            Mock<IProductCategoryService>? productCategoryService = null,
            Mock<IProductStatusService>? productStatusService = null,
            Mock<IProductWebFormService>? productWebFormService = null,
            int? userId = null)
        {
            productService ??= new Mock<IProductService>();
            productTypeService ??= new Mock<IProductTypeService>();
            productCategoryService ??= new Mock<IProductCategoryService>();
            productStatusService ??= new Mock<IProductStatusService>();
            productWebFormService ??= new Mock<IProductWebFormService>();

            var controller = new ProductController(
                productService.Object,
                productTypeService.Object,
                productCategoryService.Object,
                productStatusService.Object,
                productWebFormService.Object)
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
        public async Task CreateProduct_Should_ReturnBadRequest_When_ModelStateInvalid()
        {
            var controller = CreateController(userId: 1);
            controller.ModelState.AddModelError("ProductID", "Required");

            var result = await controller.CreateProduct(new CreateProductRequestDto());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetProductById_Should_ReturnNotFound_When_ResponseIsNull()
        {
            var productService = new Mock<IProductService>();
            productService.Setup(s => s.GetProductByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ProductFullResponseDto?)null);

            var controller = CreateController(productService);

            var result = await controller.GetProductById(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task PublishProduct_Should_ReturnBadRequest_When_ProductIdsMissing()
        {
            var controller = CreateController(userId: 5);

            var result = await controller.PublishProduct(new PublishProductsRequestDto { ProductIds = new List<Guid>() });

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateProduct_Should_CallService_When_UserPresent()
        {
            var productService = new Mock<IProductService>();
            productService.Setup(s => s.CreateProductAsync(It.IsAny<CreateProductRequestDto>(), It.IsAny<int>()))
                .ReturnsAsync(new CreateProductResponseDto { Id = Guid.NewGuid() });

            var controller = CreateController(productService, userId: 3);

            var result = await controller.CreateProduct(new CreateProductRequestDto { Name = "Test", ProductID = "PID" });

            result.Should().BeOfType<OkObjectResult>();
            productService.Verify(s => s.CreateProductAsync(It.IsAny<CreateProductRequestDto>(), 3), Times.Once);
        }
    }
}