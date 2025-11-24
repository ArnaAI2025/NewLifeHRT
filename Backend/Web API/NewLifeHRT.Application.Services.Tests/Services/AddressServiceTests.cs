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
    public class AddressServiceBuilder : ServiceBuilder<AddressService>
    {
        public override AddressService Build()
        {
            return new AddressService(AddressRepositoryMock.Object);
        }
    }

    public class AddressServiceTests
    {
        [Fact]
        public async Task CreateAddressAsync_Should_ReturnMessage_When_AddressMissing()
        {
            // Arrange
            var addressService = new AddressServiceBuilder().Build();

            // Act
            var response = await addressService.CreateAddressAsync(null, 1);

            // Assert
            response.Id.Should().Be(Guid.Empty);
            response.Message.Should().Be("Address details are required.");
        }

        [Fact]
        public async Task UpdateAddressAsync_Should_ReturnMessage_When_IdMissing()
        {
            // Arrange
            var addressService = new AddressServiceBuilder().Build();

            // Act
            var response = await addressService.UpdateAddressAsync(Guid.Empty, new AddressDto(), 1);

            // Assert
            response.Id.Should().Be(Guid.Empty);
            response.Message.Should().Be("Existing address identifier is required.");
        }

        [Fact]
        public async Task BulkToggleActiveAsync_Should_ReturnMessage_When_NoIds()
        {
            // Arrange
            var addressService = new AddressServiceBuilder().Build();

            // Act
            var response = await addressService.BulkToggleActiveAsync(null, 1, true);

            // Assert
            response.Message.Should().Be("No address IDs provided.");
            response.SuccessCount.Should().Be(0);
            response.FailedCount.Should().Be(0);
        }
    }
}