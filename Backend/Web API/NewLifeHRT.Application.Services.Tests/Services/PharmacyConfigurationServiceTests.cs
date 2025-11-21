using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Settings;
using NewLifeHRT.Tests.Common.Builders;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class PharmacyConfigurationServiceBuilder : ServiceBuilder<PharmacyConfigurationService>
    {
        public override PharmacyConfigurationService Build()
        {
            return new PharmacyConfigurationService(
                IntegrationTypeRepositoryMock.Object,
                IntegrationKeyRepositoryMock.Object,
                PharmacyConfigurationRepositoryMock.Object,
                PharmacyConfigurationDataRepositoryMock.Object,
                Options.Create(new SecuritySettings { Key = "1234567890123456", IV = "1234567890123456" }));
        }
    }

    public class PharmacyConfigurationServiceTests
    {
        [Fact]
        public async Task CreatePharmacyConfigurationAsync_Should_ReturnMessage_When_ConfigMissing()
        {
            var service = new PharmacyConfigurationServiceBuilder().Build();

            var response = await service.CreatePharmacyConfigurationAsync(new PharmacyConfigurationRequestDto
            {
                PharmacyId = Guid.NewGuid(),
                TypeId = 1,
                ConfigData = Array.Empty<PharmacyConfigurationDataRequestDto>()
            }, 1);

            response.Message.Should().Contain("Configuration data is required");
        }

        [Fact]
        public async Task ActivatePharmacyConfigurationsAsync_Should_IncrementSuccessCount()
        {
            var ids = new List<Guid> { Guid.NewGuid() };
            var repo = new Mock<IPharmacyConfigurationRepository>();
            repo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PharmacyConfigurationEntity, bool>>>(), false))
                .ReturnsAsync(ids.Select(id => new PharmacyConfigurationEntity(Guid.NewGuid(), 1, null, DateTime.UtcNow) { Id = id }));

            var service = new PharmacyConfigurationServiceBuilder()
                .SetParameter(repo)
                .Build();

            var result = await service.ActivatePharmacyConfigurationsAsync(ids, 5);

            result.SuccessCount.Should().Be(1);
            repo.Verify(r => r.UpdateAsync(It.IsAny<PharmacyConfigurationEntity>()), Times.Once);
        }
    }
}
