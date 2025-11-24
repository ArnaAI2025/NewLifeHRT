using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Tests.Common.Builders;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class LeadServiceBuilder : ServiceBuilder<LeadService>
    {
        public override LeadService Build()
        {
            return new LeadService(LeadRepositoryMock.Object, AddressRepositoryMock.Object, ClinicDbContextMock.Object);
        }
    }

    public class LeadServiceTests
    {
        [Fact]
        public async Task CreateAsync_Should_CreateLead_WithAddress()
        {
            var addressId = Guid.NewGuid();
            var leadRepository = new Mock<ILeadRepository>();
            Lead? savedLead = null;
            leadRepository.Setup(r => r.AddAsync(It.IsAny<Lead>())).Callback<Lead>(l => savedLead = l).ReturnsAsync((Lead l) => l);

            var addressRepository = new Mock<IAddressRepository>();
            addressRepository.Setup(r => r.AddAsync(It.IsAny<Address>())).ReturnsAsync(new Address { Id = addressId });

            var request = new LeadRequestDto
            {
                Subject = "test",
                FirstName = "First",
                LastName = "Last",
                PhoneNumber = "123",
                Email = "a@b.com",
                OwnerId = 1,
                AddressDto = new AddressDto { AddressLine1 = "123", City = "City", StateId = 1, CountryId = 1, PostalCode = "000" }
            };

            var service = new LeadServiceBuilder()
                .SetParameter(leadRepository)
                .SetParameter(addressRepository)
                .Build();

            var response = await service.CreateAsync(request, 99);

            response.Id.Should().NotBeNull();
            response.Message.Should().Be("Lead created suceessfully");
            savedLead.Should().NotBeNull();
            savedLead!.AddressId.Should().Be(addressId);
        }

        [Fact]
        public async Task BulkToggleActiveStatusAsync_Should_ReturnMessage_When_NoIdsProvided()
        {
            var service = new LeadServiceBuilder().Build();

            var response = await service.BulkToggleActiveStatusAsync(null!, 1, true);

            response.Message.Should().Be("No valid Lead IDs provided.");
        }

        [Fact]
        public async Task BulkDeleteLeadsAsync_Should_Throw_When_NoLeadsFound()
        {
            var leadRepository = new Mock<ILeadRepository>();
            leadRepository.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Lead, bool>>>(), It.IsAny<bool>()))
                .ReturnsAsync(Enumerable.Empty<Lead>());

            var service = new LeadServiceBuilder().SetParameter(leadRepository).Build();

            await Assert.ThrowsAsync<Exception>(() => service.BulkDeleteLeadsAsync(new List<Guid> { Guid.NewGuid() }, 1));
        }
    }
}