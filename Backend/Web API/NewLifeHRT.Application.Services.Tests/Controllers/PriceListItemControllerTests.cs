using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class PriceListItemControllerTests
    {
        private static PriceListItemController CreateController(
            Mock<IPriceListItemService>? priceListItemService = null,
            Mock<ILifeFileScheduleCodeService>? scheduleCodeService = null,
            Mock<ILifeFileDrugFormService>? drugFormService = null,
            Mock<ILifeFileQuantityUnitService>? quantityUnitService = null,
            int? userId = null)
        {
            priceListItemService ??= new Mock<IPriceListItemService>();
            scheduleCodeService ??= new Mock<ILifeFileScheduleCodeService>();
            drugFormService ??= new Mock<ILifeFileDrugFormService>();
            quantityUnitService ??= new Mock<ILifeFileQuantityUnitService>();

            var controller = new PriceListItemController(
                priceListItemService.Object,
                scheduleCodeService.Object,
                drugFormService.Object,
                quantityUnitService.Object)
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
        public async Task CreatePriceListItem_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = CreateController();

            var result = await controller.CreatePriceListItem(new PriceListItemRequestDto());

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task DeletePriceListItem_Should_ReturnBadRequest_When_NoIds()
        {
            var controller = CreateController(userId: 4);

            var result = await controller.DeletePriceListItem(new PriceListItemActionsRequestDto());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetPriceListItemById_Should_ReturnNotFound_When_ServiceReturnsNull()
        {
            var service = new Mock<IPriceListItemService>();
            service.Setup(s => s.GetPriceListItemByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Application.Services.Models.Response.PriceListItemGetByIdResponseDto?)null);

            var controller = CreateController(service);

            var result = await controller.GetPriceListItemById(Guid.NewGuid());

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task ActivatePriceListItem_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = CreateController();

            var result = await controller.ActivatePriceListItem(new PriceListItemActionsRequestDto { PriceListItemIds = new List<Guid> { Guid.NewGuid() } });

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}