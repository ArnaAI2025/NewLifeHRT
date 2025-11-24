using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.DTOs;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using System;
using System.Collections.Generic;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class ProposalDetailServiceBuilder : ServiceBuilder<ProposalDetailService>
    {
        public Mock<IProposalDetailRepository> ProposalDetailRepositoryMock { get; private set; } = new();
        public Mock<IPriceListItemService> PriceListItemServiceMock { get; private set; } = new();

        public ProposalDetailServiceBuilder SetParameter(Mock<IProposalDetailRepository> repository)
        {
            ProposalDetailRepositoryMock = repository;
            return this;
        }

        public ProposalDetailServiceBuilder SetParameter(Mock<IPriceListItemService> priceListItemService)
        {
            PriceListItemServiceMock = priceListItemService;
            return this;
        }

        public override ProposalDetailService Build()
        {
            return new ProposalDetailService(ProposalDetailRepositoryMock.Object, PriceListItemServiceMock.Object);
        }
    }

    public class ProposalDetailServiceTests
    {
        [Fact]
        public async Task CreateProposalDetailAsync_Should_ReturnSuccess_When_DetailsCreated()
        {
            var proposalDetailRepository = new Mock<IProposalDetailRepository>();
            proposalDetailRepository.Setup(r => r.AddRangeAsync(It.IsAny<List<ProposalDetail>>())).Returns(Task.CompletedTask);

            var priceListItemService = new Mock<IPriceListItemService>();
            var priceListItemId = Guid.NewGuid();
            priceListItemService.Setup(s => s.GetPricesByIdsAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new Dictionary<Guid, decimal> { { priceListItemId, 10m } });

            var dtoList = new List<ProposalDetailRequestDto>
            {
                new()
                {
                    ProductPharmacyPriceListItemId = priceListItemId,
                    ProductId = Guid.NewGuid(),
                    Quantity = 1,
                    Amount = 10m,
                    PerUnitAmount = 10m
                }
            };

            var service = new ProposalDetailServiceBuilder()
                .SetParameter(proposalDetailRepository)
                .SetParameter(priceListItemService)
                .Build();

            var response = await service.CreateProposalDetailAsync(dtoList, Guid.NewGuid(), 1);

            response.SuccessCount.Should().Be(1);
            response.Message.Should().Contain("successfully");
        }

        [Fact]
        public async Task BulkToggleActiveStatusAsync_Should_ReturnMessage_When_NoIdsProvided()
        {
            var service = new ProposalDetailServiceBuilder().Build();

            var response = await service.BulkToggleActiveStatusAsync(null!, 1, true);

            response.Message.Should().Be("No valid proposal detail IDs provided.");
        }
    }
}