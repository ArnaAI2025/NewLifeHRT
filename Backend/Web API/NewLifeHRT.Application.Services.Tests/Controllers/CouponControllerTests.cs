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
    public class CouponControllerTests
    {
        private static CouponController CreateController(Mock<ICouponService>? couponService = null, int? userId = null)
        {
            couponService ??= new Mock<ICouponService>();

            var controller = new CouponController(couponService.Object)
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
        public async Task GetAllAsync_Should_ReturnOk_WithCoupons()
        {
            var coupons = new List<CouponResponseDto> { new(), new() };
            var couponService = new Mock<ICouponService>();
            couponService.Setup(s => s.GetAllAsync()).ReturnsAsync(coupons);

            var controller = CreateController(couponService);

            var result = await controller.GetAllAsync();

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(coupons);
        }

        [Fact]
        public async Task Create_Should_PassUserId_ToService()
        {
            var response = new CommonOperationResponseDto<Guid> { Id = Guid.NewGuid() };
            var couponService = new Mock<ICouponService>();
            couponService.Setup(s => s.Create(It.IsAny<CouponRequestDto>(), It.IsAny<int>()))
                .ReturnsAsync(response);

            var controller = CreateController(couponService, userId: 12);

            var result = await controller.Create(new CouponRequestDto());

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().Be(response);
            couponService.Verify(s => s.Create(It.IsAny<CouponRequestDto>(), 12), Times.Once);
        }

        [Fact]
        public async Task BulkToggleActiveStatus_Should_CallService_ForActivate()
        {
            var request = new BulkOperationRequestDto<Guid> { Ids = new List<Guid> { Guid.NewGuid() } };
            var response = new BulkOperationResponseDto();
            var couponService = new Mock<ICouponService>();
            couponService.Setup(s => s.BulkToggleActiveStatusAsync(request.Ids, 3, true))
                .ReturnsAsync(response);

            var controller = CreateController(couponService, userId: 3);

            var result = await controller.BulkToggleActiveStatus(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().Be(response);
            couponService.Verify(s => s.BulkToggleActiveStatusAsync(request.Ids, 3, true), Times.Once);
        }

        [Fact]
        public async Task BulkDelete_Should_ForwardRequestToService()
        {
            var request = new BulkOperationRequestDto<Guid> { Ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() } };
            var response = new BulkOperationResponseDto();
            var couponService = new Mock<ICouponService>();
            couponService.Setup(s => s.BulkDeleteProposalAsync(request.Ids)).ReturnsAsync(response);

            var controller = CreateController(couponService);

            var result = await controller.BulkDelete(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().Be(response);
            couponService.Verify(s => s.BulkDeleteProposalAsync(request.Ids), Times.Once);
        }
    }
}