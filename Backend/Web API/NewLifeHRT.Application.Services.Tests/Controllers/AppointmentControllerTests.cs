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
    public class AppointmentControllerTests
    {
        private readonly Mock<ISlotService> _slotServiceMock = new();
        private readonly Mock<IAppointmentModeService> _appointmentModeServiceMock = new();
        private readonly Mock<IAppointmentService> _appointmentServiceMock = new();

        [Fact]
        public async Task GetAllSlots_Should_ReturnBadRequest_When_ParamsInvalid()
        {
            var controller = new AppointmentController(_slotServiceMock.Object, _appointmentModeServiceMock.Object, _appointmentServiceMock.Object);

            var result = await controller.GetAllSlots(Guid.Empty, 0, default);

            result.Should().BeOfType<BadRequestObjectResult>();
            _slotServiceMock.Verify(s => s.GetAllSlotsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<DateOnly>()), Times.Never);
        }

        [Fact]
        public async Task CreateAppointment_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = new AppointmentController(_slotServiceMock.Object, _appointmentModeServiceMock.Object, _appointmentServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            var result = await controller.CreateAppointment(new CreateAppointmentRequestDto { PatientId = Guid.NewGuid(), DoctorId = 1 });

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task CreateAppointment_Should_ReturnOk_When_ServiceSucceeds()
        {
            var successResponse = new AppointmentResultResponseDto
            {
                Success = true,
                AppointmentId = Guid.NewGuid(),
                Message = "ok"
            };

            _appointmentServiceMock.Setup(s => s.CreateAppointmentAsync(It.IsAny<CreateAppointmentRequestDto>(), 5))
                .ReturnsAsync(successResponse);

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "5") }))
            };
            var controller = new AppointmentController(_slotServiceMock.Object, _appointmentModeServiceMock.Object, _appointmentServiceMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };

            var result = await controller.CreateAppointment(new CreateAppointmentRequestDto
            {
                PatientId = Guid.NewGuid(),
                DoctorId = 1,
                SlotId = Guid.NewGuid(),
                AppointmentDate = DateOnly.FromDateTime(DateTime.Today)
            });

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(new { Message = successResponse.Message, AppointmentId = successResponse.AppointmentId });
        }
    }
}
