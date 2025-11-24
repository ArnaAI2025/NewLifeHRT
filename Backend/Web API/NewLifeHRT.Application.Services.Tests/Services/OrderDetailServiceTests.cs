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
    public class OrderDetailServiceBuilder : ServiceBuilder<OrderDetailService>
    {
        public Mock<IOrderDetailRepository> OrderDetailRepositoryMock { get; private set; } = new();

        public override OrderDetailService Build()
        {
            return new OrderDetailService(OrderDetailRepositoryMock.Object);
        }

        public OrderDetailServiceBuilder SetParameter(Mock<IOrderDetailRepository> repository)
        {
            OrderDetailRepositoryMock = repository;
            return this;
        }
    }

    public class OrderDetailServiceTests
    {
        [Fact]
        public async Task CreateOrderDetailAsync_Should_ThrowException_When_DetailsNull()
        {
            var service = new OrderDetailServiceBuilder().Build();

            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateOrderDetailAsync(Guid.NewGuid(), null!, 1));
        }

        [Fact]
        public async Task UpdateOrderDetailAsync_Should_DeleteMissingExistingRows()
        {
            // Arrange
            var existingId = Guid.NewGuid();
            var repositoryMock = new Mock<IOrderDetailRepository>();
            repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrderDetail, bool>>>(), false))
                .ReturnsAsync(new List<OrderDetail> { new OrderDetail { Id = existingId, OrderId = Guid.NewGuid() } });

            repositoryMock.Setup(r => r.DeleteAsync(It.Is<OrderDetail>(od => od.Id == existingId)))
                .Returns(Task.CompletedTask);

            var service = new OrderDetailServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            // Act
            var response = await service.UpdateOrderDetailAsync(Guid.NewGuid(), null, 1);

            // Assert
            response.SuccessCount.Should().Be(1);
            response.FailedCount.Should().Be(0);
            response.SuccessIds.Should().Contain(existingId.ToString());
            repositoryMock.Verify(r => r.DeleteAsync(It.Is<OrderDetail>(od => od.Id == existingId)), Times.Once);
        }
    }
}