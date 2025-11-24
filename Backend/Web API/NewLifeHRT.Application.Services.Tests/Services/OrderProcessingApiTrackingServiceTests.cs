using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class OrderProcessingApiTrackingServiceBuilder : ServiceBuilder<OrderProcessingApiTrackingService>
    {
        public Mock<IOrderProcessingApiTrackingRepository> OrderProcessingApiTrackingRepositoryMock { get; private set; } = new();

        public override OrderProcessingApiTrackingService Build()
        {
            return new OrderProcessingApiTrackingService(OrderProcessingApiTrackingRepositoryMock.Object);
        }

        public OrderProcessingApiTrackingServiceBuilder SetParameter(Mock<IOrderProcessingApiTrackingRepository> repository)
        {
            OrderProcessingApiTrackingRepositoryMock = repository;
            return this;
        }
    }

    public class OrderProcessingApiTrackingServiceTests
    {
        [Fact]
        public async Task GetErrorTrackingsAsync_Should_ReturnMappedErrors()
        {
            // Arrange
            var trackingRepository = new Mock<IOrderProcessingApiTrackingRepository>();
            var tracking = new OrderProcessingApiTracking
            {
                OrderId = Guid.NewGuid(),
                Order = new Order { Name = "Order-1", Status = OrderStatus.LifeFileError, Pharmacy = new Pharmacy { Name = "Pharm" } },
                IntegrationType = new IntegrationType { Type = "TypeA" },
                Transactions = new List<OrderProcessingApiTransaction>
                {
                    new OrderProcessingApiTransaction
                    {
                        Endpoint = "endpoint",
                        Payload = "payload",
                        ResponseMessage = "error",
                        Status = OrderProcessingApiTrackingStatusEnum.Failed
                    },
                    new OrderProcessingApiTransaction { Status = OrderProcessingApiTrackingStatusEnum.Success }
                }
            };

            trackingRepository.Setup(r => r.FindWithIncludeAsync(
                    It.IsAny<List<System.Linq.Expressions.Expression<Func<OrderProcessingApiTracking, bool>>>>(),
                    It.IsAny<string[]>(),
                    true))
                .ReturnsAsync(new List<OrderProcessingApiTracking> { tracking });

            var service = new OrderProcessingApiTrackingServiceBuilder()
                .SetParameter(trackingRepository)
                .Build();

            // Act
            var result = await service.GetErrorTrackingsAsync();

            // Assert
            result.Should().HaveCount(1);
            result[0].OrderName.Should().Be("Order-1");
            result[0].PharmacyName.Should().Be("Pharm");
            result[0].IntegrationType.Should().Be("TypeA");
            result[0].Transactions.Should().HaveCount(1);
            trackingRepository.Verify(r => r.FindWithIncludeAsync(
                It.IsAny<List<System.Linq.Expressions.Expression<Func<OrderProcessingApiTracking, bool>>>>(),
                It.Is<string[]>(includes => includes.Length == 4),
                true), Times.Once);
        }
    }
}