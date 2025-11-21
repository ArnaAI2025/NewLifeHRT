using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class AppointmentModeServiceBuilder : ServiceBuilder<AppointmentModeService>
    {
        public override AppointmentModeService Build()
        {
            return new AppointmentModeService(AppointmentModeRepositoryMock.Object);
        }
    }

    public class AppointmentModeServiceTests
    {
        [Fact]
        public async Task GetAllAppointmentModesAsync_Should_ReturnModes()
        {
            var modes = new List<AppointmentMode> { new(), new() };
            var repo = new Mock<IAppointmentModeRepository>();
            repo.Setup(r => r.GetAllAsync()).ReturnsAsync(modes);

            var service = new AppointmentModeServiceBuilder()
                .SetParameter(repo)
                .Build();

            var result = await service.GetAllAppointmentModesAsync();

            result.Should().HaveCount(2);
        }
    }
}
