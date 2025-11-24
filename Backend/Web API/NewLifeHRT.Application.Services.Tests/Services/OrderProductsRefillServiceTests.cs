using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class OrderProductsRefillServiceBuilder : ServiceBuilder<OrderProductsRefillService>
    {
        public Mock<IOrderProductsRefillRepository> OrderProductsRefillRepositoryMock { get; private set; } = new();

        public override OrderProductsRefillService Build()
        {
            return new OrderProductsRefillService(OrderProductsRefillRepositoryMock.Object);
        }

        public OrderProductsRefillServiceBuilder SetParameter(Mock<IOrderProductsRefillRepository> repository)
        {
            OrderProductsRefillRepositoryMock = repository;
            return this;
        }
    }

    public class OrderProductsRefillServiceTests
    {
        [Fact]
        public async Task DeleteOrderProductRefillRecordsAsync_Should_ReturnZero_When_IdsEmpty()
        {
            var service = new OrderProductsRefillServiceBuilder().Build();

            var result = await service.DeleteOrderProductRefillRecordsAsync(new List<Guid>());

            result.Should().Be(0);
        }

        [Fact]
        public async Task UpdateOrderProductRefillDetailAsync_Should_ReturnFalse_When_EntityMissing()
        {
            var repository = new Mock<IOrderProductsRefillRepository>();
            repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((OrderProductRefillDetail?)null);

            var service = new OrderProductsRefillServiceBuilder()
                .SetParameter(repository)
                .Build();

            var result = await service.UpdateOrderProductRefillDetailAsync(Guid.NewGuid(), new UpdateOrderProductRefillDetailRequestDto(), 1);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetOrderProductRefillByIdAsync_Should_ReturnNull_When_EntityInactive()
        {
            var repository = new Mock<IOrderProductsRefillRepository>();
            repository.Setup(r => r.GetWithIncludeAsync(It.IsAny<Guid>(), It.IsAny<string[]>()))
                .ReturnsAsync(new OrderProductRefillDetail { Id = Guid.NewGuid(), IsActive = false });

            var service = new OrderProductsRefillServiceBuilder()
                .SetParameter(repository)
                .Build();

            var result = await service.GetOrderProductRefillByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }
    }
}