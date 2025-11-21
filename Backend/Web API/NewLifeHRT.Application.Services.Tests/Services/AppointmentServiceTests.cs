using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class AppointmentServiceBuilder : ServiceBuilder<AppointmentService>
    {
        public override AppointmentService Build()
        {
            return new AppointmentService(
                AppointmentRepositoryMock.Object,
                HolidayRepositoryMock.Object,
                SingletonServiceMock.Object,
                SlotRepositoryMock.Object);
        }
    }

    public class AppointmentServiceTests
    {
        [Fact]
        public async Task CreateAppointmentAsync_Should_ReturnFailure_When_SlotMissing()
        {
            var slotRepo = new Mock<ISlotRepository>();
            slotRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Slot?)null);

            var service = new AppointmentServiceBuilder()
                .SetParameter(slotRepo)
                .Build();

            var result = await service.CreateAppointmentAsync(new CreateAppointmentRequestDto
            {
                SlotId = Guid.NewGuid(),
                DoctorId = 1,
                PatientId = Guid.NewGuid(),
                AppointmentDate = DateOnly.FromDateTime(DateTime.Today)
            }, 1);

            result.Success.Should().BeFalse();
            result.Message.Should().Be("Slot not found.");
        }

        [Fact]
        public async Task DeleteAppointmentAsync_Should_ReturnSuccess_When_AppointmentExists()
        {
            var appointment = new Appointment();
            var appointmentRepo = new Mock<IAppointmentRepository>();
            appointmentRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(appointment);

            var service = new AppointmentServiceBuilder()
                .SetParameter(appointmentRepo)
                .Build();

            var response = await service.DeleteAppointmentAsync(Guid.NewGuid());

            response.Success.Should().BeTrue();
            appointmentRepo.Verify(r => r.DeleteAsync(appointment), Times.Once);
        }
    }
}
