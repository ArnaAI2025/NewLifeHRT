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
    public class OrderControllerTests
    {
        private readonly Mock<IOrderService> _orderServiceMock = new();
        private readonly Mock<IOrderProcessingApiTrackingService> _orderProcessingApiTrackingService = new();

        [Fact]
        public async Task GetById_Should_ReturnNotFound_When_ServiceReturnsNull()
        {
            _orderServiceMock.Setup(s => s.GetOrderByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((OrderResponseDto?)null);

            var controller = new OrderController(_orderServiceMock.Object, _orderProcessingApiTrackingService.Object);

            var result = await controller.GetById(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = new OrderController(_orderServiceMock.Object, _orderProcessingApiTrackingService.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            var result = await controller.Create(new OrderRequestDto());

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task BulkDelete_Should_ReturnBadRequest_When_NoIdsProvided()
        {
            var controller = new OrderController(_orderServiceMock.Object, _orderProcessingApiTrackingService.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            var result = await controller.BulkDelete(null!);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetReceiptById_Should_ReturnNotFound_When_ServiceReturnsNull()
        {
            _orderServiceMock.Setup(s => s.GetPrescriptionReceiptDataAsync(It.IsAny<Guid>(), It.IsAny<bool?>(), true))
                .ReturnsAsync((PrescriptionReceiptDto?)null);

            var controller = new OrderController(_orderServiceMock.Object, _orderProcessingApiTrackingService.Object);

            var result = await controller.GetReceiptById(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AcceptOrder_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = new OrderController(_orderServiceMock.Object, _orderProcessingApiTrackingService.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            var result = await controller.AcceptOrder(Guid.NewGuid());

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}