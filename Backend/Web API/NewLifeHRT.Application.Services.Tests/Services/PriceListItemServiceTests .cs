using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using NewLifeHRT.Tests.Common.Fixtures;
using NewLifeHRT.Tests.Common.Tests;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class PriceListItemServiceBuilder : ServiceBuilder<PriceListItemService>
    {
        public override PriceListItemService Build()
        {
            return new PriceListItemService(PriceListItemRepositoryMock.Object);
        }
    }

    public class PriceListItemServiceTests : BaseServiceTest, IClassFixture<DatabaseFixture>
    {
        public PriceListItemServiceTests(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task CreatePriceListItemAsync_Should_CallAddAsync()
        {
            var repositoryMock = new Mock<IPriceListItemRepository>();
            repositoryMock.Setup(r => r.AddAsync(It.IsAny<ProductPharmacyPriceListItem>()))
                .ReturnsAsync((ProductPharmacyPriceListItem p) =>
                {
                    p.Id = Guid.NewGuid();
                    return p;
                });

            var request = new PriceListItemRequestDto
            {
                CurrencyId = 1,
                Amount = 25m,
                CostOfProduct = 10m,
                LifeFilePharmacyProductId = "2",
                LifeFielForeignPmsId = "3",
                LifeFileDrugFormId = 4,
                LifeFileDrugName = "drug",
                LifeFileDrugStrength = "10mg",
                LifeFileQuantityUnitId = 5,
                LifeFileScheduleCodeId = 6,
                PharmacyId = Guid.NewGuid(),
                ProductId = Guid.NewGuid()
            };

            var service = new PriceListItemServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var result = await service.CreatePriceListItemAsync(request, 7);

            result.Id.Should().NotBe(Guid.Empty);
            repositoryMock.Verify(r => r.AddAsync(It.IsAny<ProductPharmacyPriceListItem>()), Times.Once);
        }

        [Fact]
        public async Task ActivatePriceListItemAsync_Should_Throw_When_NoItemsFound()
        {
            var repositoryMock = new Mock<IPriceListItemRepository>();
            repositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ProductPharmacyPriceListItem, bool>>>(), false))
                .ReturnsAsync(Enumerable.Empty<ProductPharmacyPriceListItem>());

            var service = new PriceListItemServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            await Assert.ThrowsAsync<Exception>(() => service.ActivatePriceListItemAsync(new List<Guid> { Guid.NewGuid() }, 1));
        }

        [Fact]
        public async Task GetPricesByIdsAsync_Should_ReturnAmountsDictionary()
        {
            var firstId = Guid.NewGuid();
            var secondId = Guid.NewGuid();
            var repositoryMock = new Mock<IPriceListItemRepository>();
            repositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ProductPharmacyPriceListItem, bool>>>(), true))
                .ReturnsAsync(new List<ProductPharmacyPriceListItem>
                {
                    new ProductPharmacyPriceListItem(
                                currencyId: null,
                                amount: 12m,
                                costOfProduct: null,
                                lifeFilePharmacyProductId: null,
                                lifeFielForeignPmsId: null,
                                lifeFileDrugFormId: null,
                                lifeFileDrugName: null,
                                lifeFileDrugStrength: null,
                                lifeFileQuantityUnitId: null,
                                lifeFileScheduledCodeId: null,
                                pharmacyId: Guid.NewGuid(),
                                productId: Guid.NewGuid(),
                                createdAt: DateTime.UtcNow,
                                createdBy: "test")
                                {
                                   Id = firstId
                                },
                    new ProductPharmacyPriceListItem(
                                currencyId: null,
                                amount: 20m,
                                costOfProduct: null,
                                lifeFilePharmacyProductId: null,
                                lifeFielForeignPmsId: null,
                                lifeFileDrugFormId: null,
                                lifeFileDrugName: null,
                                lifeFileDrugStrength: null,
                                lifeFileQuantityUnitId: null,
                                lifeFileScheduledCodeId: null,
                                pharmacyId: Guid.NewGuid(),
                                productId: Guid.NewGuid(),
                                createdAt: DateTime.UtcNow,
                                createdBy: "test")
                                {
                                    Id = secondId
                                }
                });

            var service = new PriceListItemServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var prices = await service.GetPricesByIdsAsync(new List<Guid> { firstId, secondId });

            prices.Should().ContainKey(firstId);
            prices[firstId].Should().Be(12m);
            prices[secondId].Should().Be(20m);
        }
    }
}