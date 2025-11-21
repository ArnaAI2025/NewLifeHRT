using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class PharmacyShippingMethodServiceBuilder : ServiceBuilder<PharmacyShippingMethodService>
    {
        public override PharmacyShippingMethodService Build()
        {
            return new PharmacyShippingMethodService(PharmacyShippingMethodRepositoryMock.Object);
        }
    }

    public class PharmacyShippingMethodServiceTests
    {
        [Fact]
        public async Task CreatePharmacyShippingMethodAsync_Should_ReturnMessage_When_RequestEmpty()
        {
            var service = new PharmacyShippingMethodServiceBuilder().Build();

            var response = await service.CreatePharmacyShippingMethodAsync(Array.Empty<PharmactShippingMethodRequestDto>(), Guid.NewGuid(), 1);

            response.Message.Should().Contain("No shipping methods");
        }

        [Fact]
        public async Task SetPharmacyShippingMethodsActivationStatusAsync_Should_Throw_When_NoMethodsFound()
        {
            var repo = new Mock<IPharmacyShippingMethodRepository>();
            repo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<PharmacyShippingMethod, bool>>>(), false))
                .ReturnsAsync(Enumerable.Empty<PharmacyShippingMethod>());

            var service = new PharmacyShippingMethodServiceBuilder()
                .SetParameter(repo)
                .Build();

            await Assert.ThrowsAsync<Exception>(() => service.SetPharmacyShippingMethodsActivationStatusAsync(new List<Guid> { Guid.NewGuid() }, true, 1));
        }
    }
}
