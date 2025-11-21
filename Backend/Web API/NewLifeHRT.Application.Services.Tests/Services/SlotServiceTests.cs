using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class SlotServiceBuilder : ServiceBuilder<SlotService>
    {
        public override SlotService Build()
        {
            return new SlotService(
                SlotRepositoryMock.Object,
                AppointmentRepositoryMock.Object,
                HolidayRepositoryMock.Object,
                UserRepositoryMock.Object);
        }
    }

    public class SlotServiceTests
    {
        [Fact]
        public async Task GetAllSlotsAsync_Should_Throw_When_DoctorMissing()
        {
            var userRepo = new Mock<IUserRepository>();
            userRepo.Setup(r => r.GetWithIncludeAsync(It.IsAny<int>(), It.IsAny<string[]>()))
                .ReturnsAsync((ApplicationUser?)null);

            var service = new SlotServiceBuilder()
                .SetParameter(userRepo)
                .Build();

            await Assert.ThrowsAsync<Exception>(() => service.GetAllSlotsAsync(Guid.NewGuid(), 1, DateOnly.FromDateTime(DateTime.Today)));
        }
    }
}
