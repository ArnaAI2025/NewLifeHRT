using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class ProposalServiceBuilder : ServiceBuilder<ProposalService>
    {
        public Mock<IProposalRepository> ProposalRepositoryMock { get; private set; } = new();
        public Mock<IProposalDetailService> ProposalDetailServiceMock { get; private set; } = new();
        public Mock<IOrderService> OrderServiceMock { get; private set; } = new();
        public Mock<IPatientCreditCardRepository> PatientCreditCardRepositoryMock { get; private set; } = new();

        public ProposalServiceBuilder SetParameter(Mock<IProposalRepository> repository)
        {
            ProposalRepositoryMock = repository;
            return this;
        }

        public ProposalServiceBuilder SetParameter(Mock<IProposalDetailService> detailService)
        {
            ProposalDetailServiceMock = detailService;
            return this;
        }

        public ProposalServiceBuilder SetParameter(Mock<IOrderService> orderService)
        {
            OrderServiceMock = orderService;
            return this;
        }

        public ProposalServiceBuilder SetParameter(Mock<IPatientCreditCardRepository> patientCreditCardRepository)
        {
            PatientCreditCardRepositoryMock = patientCreditCardRepository;
            return this;
        }

        public override ProposalService Build()
        {
            return new ProposalService(
                ProposalRepositoryMock.Object,
                ProposalDetailServiceMock.Object,
                ShippingAddressServiceMock.Object,
                AddressServiceMock.Object,
                OrderServiceMock.Object,
                PharmacyShippingMethodServiceMock.Object,
                PatientCreditCardRepositoryMock.Object);
        }
    }

    public class ProposalServiceTests
    {
        [Fact]
        public async Task GetProposalById_Should_ReturnNull_When_NotFound()
        {
            var proposalRepository = new Mock<IProposalRepository>();
            proposalRepository.Setup(r => r.GetWithIncludeAsync(It.IsAny<Guid>(), It.IsAny<string[]>()))
                .ReturnsAsync((Proposal?)null);

            var service = new ProposalServiceBuilder().SetParameter(proposalRepository).Build();

            var result = await service.GetProposalById(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Fact]
        public async Task BulkDeleteProposalAsync_Should_ReturnMessage_When_NoIdsProvided()
        {
            var service = new ProposalServiceBuilder().Build();

            var result = await service.BulkDeleteProposalAsync(null!);

            result.Message.Should().Be("No valid proposal IDs provided.");
        }

        [Fact]
        public async Task UpdateProposalStatusAsync_Should_ReturnMessage_When_ProposalNotFound()
        {
            var proposalRepository = new Mock<IProposalRepository>();
            proposalRepository.Setup(r => r.GetWithIncludeAsync(It.IsAny<Guid>(), It.IsAny<string[]>()))
                .ReturnsAsync((Proposal?)null);

            var service = new ProposalServiceBuilder().SetParameter(proposalRepository).Build();

            var response = await service.UpdateProposalStatusAsync(Guid.NewGuid(), 1, "", 1);

            response.Message.Should().Be("No proposals found for the provided ID.");
        }
    }
}