using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class OrderProductsRefillControllerTests
    {
        private readonly Mock<IOrderProductsRefillService> _orderProductsRefillService = new();

        [Fact]
        public async Task DeleteOrderProductRefillRecords_Should_ReturnBadRequest_When_IdsMissing()
        {
            var controller = new OrderProductsRefillController(_orderProductsRefillService.Object);

            var result = await controller.DeleteOrderProductRefillRecords(null!);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetOrderProductRefillById_Should_ReturnNotFound_When_ServiceReturnsNull()
        {
            _orderProductsRefillService.Setup(s => s.GetOrderProductRefillByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Application.Services.Models.Response.OrderProductRefillDetailByIdResponseDto?)null);

            var controller = new OrderProductsRefillController(_orderProductsRefillService.Object);

            var result = await controller.GetOrderProductRefillById(Guid.NewGuid());

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task UpdateOrderProductRefillDetail_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = new OrderProductsRefillController(_orderProductsRefillService.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            var result = await controller.UpdateOrderProductRefillDetail(Guid.NewGuid(), new UpdateOrderProductRefillDetailRequestDto());

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}