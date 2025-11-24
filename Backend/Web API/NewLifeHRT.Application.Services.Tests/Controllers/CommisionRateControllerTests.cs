using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System.Security.Claims;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class CommisionRateControllerTests
    {
        private static CommisionRateController CreateController(Mock<ICommisionRateService>? commisionRateService = null, int? userId = null)
        {
            commisionRateService ??= new Mock<ICommisionRateService>();

            var controller = new CommisionRateController(commisionRateService.Object)
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
        public async Task GetCommissionRateById_Should_ReturnNotFound_When_ServiceReturnsNull()
        {
            var commisionRateService = new Mock<ICommisionRateService>();
            commisionRateService.Setup(s => s.GetCommisionRateByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((CommisionRateGetByIdResponseDto?)null);

            var controller = CreateController(commisionRateService);

            var result = await controller.GetCommissionRateById(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateCommissionRate_Should_ReturnBadRequest_When_ModelStateInvalid()
        {
            var controller = CreateController(userId: 1);
            controller.ModelState.AddModelError("ProductId", "Required");

            var result = await controller.CreateCommissionRate(new CommisionRateRequestDto());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateCommissionRate_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = CreateController();

            var result = await controller.CreateCommissionRate(new CommisionRateRequestDto());

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task CreateCommissionRate_Should_InvokeService_When_ValidRequest()
        {
            var commisionRateService = new Mock<ICommisionRateService>();
            commisionRateService.Setup(s => s.CreateCommisionRateAsync(It.IsAny<CommisionRateRequestDto>(), It.IsAny<int>()))
                .ReturnsAsync(new CommonOperationResponseDto<Guid>());

            var controller = CreateController(commisionRateService, userId: 3);

            var result = await controller.CreateCommissionRate(new CommisionRateRequestDto { ProductId = Guid.NewGuid() });

            result.Should().BeOfType<OkObjectResult>();
            commisionRateService.Verify(s => s.CreateCommisionRateAsync(It.IsAny<CommisionRateRequestDto>(), 3), Times.Once);
        }

        [Fact]
        public async Task DeleteCommissionRate_Should_ReturnBadRequest_When_IdsEmpty()
        {
            var controller = CreateController(userId: 4);

            var result = await controller.DeleteCommissionRate(new List<Guid>());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ActivateCommissionRate_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = CreateController();

            var result = await controller.ActivateCommissionRate(new List<Guid> { Guid.NewGuid() });

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}