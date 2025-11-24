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
    public class ShippingAddressServiceBuilder : ServiceBuilder<ShippingAddressService>
    {
        public override ShippingAddressService Build()
        {
            return new ShippingAddressService(ShippingAddressRepositoryMock.Object, AddressServiceMock.Object);
        }
    }

    public class ShippingAddressServiceTests
    {
        [Fact]
        public async Task CreateAsync_Should_ReturnInvalidMessage_When_AddressMissing()
        {
            // Arrange
            var request = new ShippingAddressRequestDto();
            var shippingAddressService = new ShippingAddressServiceBuilder().Build();

            // Act
            var response = await shippingAddressService.CreateAsync(request, 1, null, false);

            // Assert
            response.Id.Should().Be(Guid.Empty);
            response.Message.Should().Be("Invalid address data");
        }

        [Fact]
        public async Task SetDefaultAsync_Should_ReturnMessage_When_NoAddresses()
        {
            // Arrange
            var shippingAddressRepositoryMock = new Mock<IShippingAddressRepository>();
            shippingAddressRepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ShippingAddress, bool>>>(), false))
                .ReturnsAsync(new List<ShippingAddress>());

            var shippingAddressService = new ShippingAddressServiceBuilder()
                .SetParameter(shippingAddressRepositoryMock)
                .Build();

            // Act
            var response = await shippingAddressService.SetDefaultAsync(Guid.NewGuid(), Guid.NewGuid(), 1);

            // Assert
            response.Id.Should().Be(Guid.Empty);
            response.Message.Should().Be("No shipping addresses found for this patient.");
        }

        [Fact]
        public async Task BulkToggleActiveAsync_Should_ReturnMessage_When_NoIds()
        {
            // Arrange
            var shippingAddressService = new ShippingAddressServiceBuilder().Build();

            // Act
            var response = await shippingAddressService.BulkToggleActiveAsync(null, 1, true);

            // Assert
            response.Message.Should().Be("No shipping address IDs provided.");
            response.SuccessCount.Should().Be(0);
            response.FailedCount.Should().Be(0);
        }
    }
}