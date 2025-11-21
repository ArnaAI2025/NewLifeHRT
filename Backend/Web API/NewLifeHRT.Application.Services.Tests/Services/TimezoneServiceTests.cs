using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class TimezoneServiceBuilder : ServiceBuilder<TimezoneService>
    {
        public override TimezoneService Build()
        {
            return new TimezoneService(TimezoneRepositoryMock.Object);
        }
    }

    public class TimezoneServiceTests
    {
        [Fact]
        public async Task GetAllAsync_Should_ReturnTimezones()
        {
            var repo = new Mock<ITimezoneRepository>();
            repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Timezone> { new(), new() });

            var service = new TimezoneServiceBuilder()
                .SetParameter(repo)
                .Build();

            var result = await service.GetAllAsync();

            result.Should().HaveCount(2);
        }
    }
}
