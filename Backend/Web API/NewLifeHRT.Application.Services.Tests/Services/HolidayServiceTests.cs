using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class HolidayServiceBuilder : ServiceBuilder<HolidayService>
    {
        public override HolidayService Build()
        {
            return new HolidayService(
                HolidayRepositoryMock.Object,
                HolidayDateRepositoryMock.Object,
                HolidayRecurrenceRepositoryMock.Object);
        }
    }

    public class HolidayServiceTests
    {
        [Fact]
        public async Task CreateHolidayAsync_Should_Throw_When_RequestNull()
        {
            var service = new HolidayServiceBuilder().Build();

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateHolidayAsync(null!, 1));
        }
    }
}
