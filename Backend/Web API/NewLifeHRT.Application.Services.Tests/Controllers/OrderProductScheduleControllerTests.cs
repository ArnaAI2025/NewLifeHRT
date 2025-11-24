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
    public class OrderProductScheduleControllerTests
    {
        private readonly Mock<IOrderProductScheduleService> _orderProductScheduleService = new();

        [Fact]
        public async Task GetSchedules_Should_ReturnBadRequest_When_RequestIsNull()
        {
            var controller = new OrderProductScheduleController(_orderProductScheduleService.Object);

            var result = await controller.GetSchedules(null!);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetSchedules_Should_ReturnBadRequest_When_EndDateBeforeStartDate()
        {
            var request = new OrderProductScheduleFilterRequestDto
            {
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1)
            };

            var controller = new OrderProductScheduleController(_orderProductScheduleService.Object);

            var result = await controller.GetSchedules(request);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetSchedules_Should_ReturnIsPatientFalse_When_UserIsNotPatient()
        {
            var request = new OrderProductScheduleFilterRequestDto
            {
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            var controller = new OrderProductScheduleController(_orderProductScheduleService.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            var result = await controller.GetSchedules(request) as OkObjectResult;

            result.Should().NotBeNull();
            result!.Value.Should().BeEquivalentTo(new
            {
                isPatient = false,
                message = "The logged-in user is not a patient.",
                schedules = new List<OrderProductScheduleResponseDto>()
            });
        }

        [Fact]
        public async Task GetScheduleSummaryById_Should_ReturnNotFound_When_ServiceReturnsNull()
        {
            _orderProductScheduleService.Setup(s => s.GetScheduleSummaryByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((OrderProductScheduleSummaryDetailResponseDto?)null);

            var controller = new OrderProductScheduleController(_orderProductScheduleService.Object);

            var result = await controller.GetScheduleSummaryById(Guid.NewGuid());

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task CreatePatientSelfReminder_Should_ReturnUnauthorized_When_PatientMissing()
        {
            var controller = new OrderProductScheduleController(_orderProductScheduleService.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            var result = await controller.CreatePatientSelfReminder(new CreatePatientSelfReminderRequestDto());

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}