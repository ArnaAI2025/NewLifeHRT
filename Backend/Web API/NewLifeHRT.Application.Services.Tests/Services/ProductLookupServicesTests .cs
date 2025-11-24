using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using NewLifeHRT.Tests.Common.Fixtures;
using NewLifeHRT.Tests.Common.Tests;
using System.Collections.Generic;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class ProductStatusServiceBuilder : ServiceBuilder<ProductStatusService>
    {
        public override ProductStatusService Build()
        {
            return new ProductStatusService(ProductStatusRepositoryMock.Object);
        }
    }

    public class ProductTypeServiceBuilder : ServiceBuilder<ProductTypeService>
    {
        public override ProductTypeService Build()
        {
            return new ProductTypeService(ProductTypeRepositoryMock.Object);
        }
    }

    public class ProductCategoryServiceBuilder : ServiceBuilder<ProductCategoryService>
    {
        public override ProductCategoryService Build()
        {
            return new ProductCategoryService(ProductCategoryRepositoryMock.Object);
        }
    }

    public class ProductWebFormServiceBuilder : ServiceBuilder<ProductWebFormService>
    {
        public override ProductWebFormService Build()
        {
            return new ProductWebFormService(ProductWebFormRepositoryMock.Object);
        }
    }

    public class ProductLookupServicesTests : BaseServiceTest, IClassFixture<DatabaseFixture>
    {
        public ProductLookupServicesTests(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
        }

        [Fact]
        public async Task ProductStatusService_Should_Return_All_Statuses()
        {
            var repositoryMock = new Mock<IProductStatusRepository>();
            repositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<ProductStatus>
                {
                    new ProductStatus { Id = 1, StatusName = "Draft" },
                    new ProductStatus { Id = 2, StatusName = "Active" }
                });

            var service = new ProductStatusServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var statuses = await service.GetAllProductStatusAsync();

            statuses.Should().HaveCount(2);
            repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task ProductTypeService_Should_Return_All_Types()
        {
            var repositoryMock = new Mock<IProductTypeRepository>();
            repositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<ProductType>
                {
                    new ProductType { Id = 1, Name = "Type1" }
                });

            var service = new ProductTypeServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var types = await service.GetAllProductTypesAsync();

            types.Should().ContainSingle();
            repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task ProductCategoryService_Should_Return_All_Categories()
        {
            var repositoryMock = new Mock<IProductCategoryRepository>();
            repositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<ProductCategory>
                {
                    new ProductCategory { Id = 1, Name = "Cat1" },
                    new ProductCategory { Id = 2, Name = "Cat2" }
                });

            var service = new ProductCategoryServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var categories = await service.GetAllProductCategoriesAsync();

            categories.Should().HaveCount(2);
            repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task ProductWebFormService_Should_Return_All_WebForms()
        {
            var repositoryMock = new Mock<IProductWebFormRepository>();
            repositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<ProductWebForm>
                {
                    new ProductWebForm { Id = 1, Name = "Form1" }
                });

            var service = new ProductWebFormServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var forms = await service.GetAllProductWebFormsAsync();

            forms.Should().ContainSingle();
            repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
}