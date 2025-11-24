using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Response;
using System.Security.Claims;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class CommissionControllerTests
    {
        private static CommissionController CreateController(
            Mock<IPoolDetailService>? poolDetailService = null,
            Mock<ICommissionsPayableService>? commissionsPayableService = null,
            int? userId = null)
        {
            poolDetailService ??= new Mock<IPoolDetailService>();
            commissionsPayableService ??= new Mock<ICommissionsPayableService>();

            var controller = new CommissionController(
                poolDetailService.Object,
                commissionsPayableService.Object)
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
        public async Task GetCounselorsByPoolDate_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = CreateController();

            var result = await controller.GetCounselorsByPoolDate();

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task GetCounselorsByPoolDate_Should_ReturnBadRequest_When_DatesMissing()
        {
            var controller = CreateController(userId: 10);

            var result = await controller.GetCounselorsByPoolDate();

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetCounselorsByPoolDate_Should_ReturnOk_WithData()
        {
            var poolDetailService = new Mock<IPoolDetailService>();
            poolDetailService.Setup(s => s.GetCounselorsByDateRangeAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .ReturnsAsync(new List<PoolDetailResponseDto> { new() });

            var controller = CreateController(poolDetailService, userId: 1);

            var result = await controller.GetCounselorsByPoolDate(DateTime.UtcNow, DateTime.UtcNow);

            result.Should().BeOfType<OkObjectResult>();
            poolDetailService.Verify(s => s.GetCounselorsByDateRangeAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Once);
        }

        [Fact]
        public async Task GetCommissionByPoolDetailId_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = CreateController();

            var result = await controller.GetCommissionByPoolDetailId(Guid.NewGuid());

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task GetCommissionByIdAsync_Should_ReturnOk_When_Authorized()
        {
            var commissionsPayableService = new Mock<ICommissionsPayableService>();
            commissionsPayableService.Setup(s => s.GetCommissionByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new CommissionsPayableDetailResponseDto());

            var controller = CreateController(commissionsPayableService: commissionsPayableService, userId: 2);

            var result = await controller.GetCommissionByIdAsync(Guid.NewGuid());

            result.Should().BeOfType<OkObjectResult>();
            commissionsPayableService.Verify(s => s.GetCommissionByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}