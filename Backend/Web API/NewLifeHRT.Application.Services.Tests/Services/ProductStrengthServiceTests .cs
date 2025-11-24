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
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class ProductStrengthServiceBuilder : ServiceBuilder<ProductStrengthService>
    {
        public override ProductStrengthService Build()
        {
            return new ProductStrengthService(ProductStrengthRepositoryMock.Object);
        }
    }

    public class ProductStrengthServiceTests : BaseServiceTest, IClassFixture<DatabaseFixture>
    {
        public ProductStrengthServiceTests(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task CreateAsync_Should_CallAddAsync_WithUserContext()
        {
            var repositoryMock = new Mock<IProductStrengthRepository>();
            repositoryMock.Setup(r => r.AddAsync(It.IsAny<ProductStrength>()))
                .ReturnsAsync((ProductStrength ps) => ps);

            var request = new ProductStrengthCreateRequestDto
            {
                ProductId = Guid.NewGuid(),
                Name = "Strength",
                Strengths = "50mg",
                Price = 15m
            };

            var service = new ProductStrengthServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var response = await service.CreateAsync(request, 2);

            response.Name.Should().Be(request.Name);
            repositoryMock.Verify(r => r.AddAsync(It.Is<ProductStrength>(ps => ps.CreatedBy == "2")), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Should_Throw_When_NotFound()
        {
            var repositoryMock = new Mock<IProductStrengthRepository>();
            repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ProductStrength?)null);

            var service = new ProductStrengthServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            await Assert.ThrowsAsync<Exception>(() => service.DeleteAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateAsync_Should_UpdateFields_AndReturnDto()
        {
            var existing = new ProductStrength(Guid.NewGuid(), "Old", "5mg", 10m, "1", DateTime.UtcNow)
            {
                Id = Guid.NewGuid()
            };

            var repositoryMock = new Mock<IProductStrengthRepository>();
            repositoryMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
            repositoryMock.Setup(r => r.UpdateAsync(existing)).ReturnsAsync(existing);

            var request = new ProductStrengthCreateRequestDto
            {
                ProductId = Guid.NewGuid(),
                Name = "Updated",
                Strengths = "10mg",
                Price = 20m
            };

            var service = new ProductStrengthServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var result = await service.UpdateAsync(existing.Id, request, 3);

            result.Name.Should().Be(request.Name);
            repositoryMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        }
    }
}