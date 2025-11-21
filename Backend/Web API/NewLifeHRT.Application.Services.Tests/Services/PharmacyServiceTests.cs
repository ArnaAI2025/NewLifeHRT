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
    public class PharmacyServiceBuilder : ServiceBuilder<PharmacyService>
    {
        public override PharmacyService Build()
        {
            return new PharmacyService(PharmacyRepositoryMock.Object, PharmacyShippingMethodServiceMock.Object);
        }
    }

    public class PharmacyServiceTests
    {
        [Fact]
        public async Task ActivatePharmaciesAsync_Should_Throw_When_NoIds()
        {
            var service = new PharmacyServiceBuilder().Build();

            await Assert.ThrowsAsync<ArgumentException>(() => service.ActivatePharmaciesAsync(new List<Guid>(), 1));
        }

        [Fact]
        public async Task CreatePharmacyAsync_Should_ReturnId_When_RequestValid()
        {
            var repo = new Mock<IPharmacyRepository>();
            var shippingService = new Mock<IPharmacyShippingMethodService>();
            var service = new PharmacyServiceBuilder()
                .SetParameter(repo)
                .SetParameter(shippingService)
                .Build();

            var response = await service.CreatePharmacyAsync(new PharmacyCreateRequestDto
            {
                Name = "Test",
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                CurrencyId = 1
            }, 5);

            response.Id.Should().NotBe(Guid.Empty);
            repo.Verify(r => r.AddAsync(It.IsAny<Pharmacy>()), Times.Once);
        }
    }
}
