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
    public class ProductServiceBuilder : ServiceBuilder<ProductService>
    {
        public override ProductService Build()
        {
            return new ProductService(ProductRepositoryMock.Object);
        }
    }

    public class ProductServiceTests : BaseServiceTest, IClassFixture<DatabaseFixture>
    {
        public ProductServiceTests(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task CreateProductAsync_Should_CallAddAndReturnId()
        {
            var repositoryMock = new Mock<IProductRepository>();
            repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Product>()))
                .ReturnsAsync((Product p) =>
                {
                    p.Id = Guid.NewGuid();
                    return p;
                });

            var request = new CreateProductRequestDto
            {
                ProductID = "PID-1",
                Name = "Product Name",
                IsColdStorageProduct = true,
                IsLabCorp = false,
                LabCode = "L1",
                ParentId = Guid.NewGuid(),
                TypeId = 1,
                Category1Id = 1,
                Category2Id = 2,
                Category3Id = 3,
                ProductDescription = "Desc",
                Protocol = "Protocol",
                IsScheduled = true,
                WebProductName = "WebName",
                WebProductDescription = "WebDesc",
                IsWebPopularMedicine = true,
                WebFormId = 2,
                WebStrength = "100mg",
                WebCost = "$10",
                IsEnabledCalculator = true,
                IsNewEnabledCalculator = false,
                IsPBPEnabled = true
            };

            var service = new ProductServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var result = await service.CreateProductAsync(request, 10);

            result.Id.Should().NotBe(Guid.Empty);
            repositoryMock.Verify(r => r.AddAsync(It.Is<Product>(p => p.CreatedBy == "10" && p.Name == request.Name)), Times.Once);
        }

        [Fact]
        public async Task GetProductByIdAsync_Should_ReturnNull_When_NotFound()
        {
            var repositoryMock = new Mock<IProductRepository>();
            repositoryMock.Setup(r => r.GetWithIncludeAsync(It.IsAny<Guid>(), It.IsAny<string[]>()))
                .ReturnsAsync((Product?)null);

            var service = new ProductServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var result = await service.GetProductByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Fact]
        public async Task PublishProductsAsync_Should_Throw_When_NoProductsFound()
        {
            var repositoryMock = new Mock<IProductRepository>();
            repositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Product, bool>>>(), false))
                .ReturnsAsync(Enumerable.Empty<Product>());

            var service = new ProductServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            await Assert.ThrowsAsync<Exception>(() => service.PublishProductsAsync(new List<Guid> { Guid.NewGuid() }, 1));
        }
    }
}