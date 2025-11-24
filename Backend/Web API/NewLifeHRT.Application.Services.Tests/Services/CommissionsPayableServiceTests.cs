using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using System.Linq.Expressions;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class CommissionsPayableServiceBuilder : ServiceBuilder<CommissionsPayableService>
    {
        public override CommissionsPayableService Build()
        {
            return new CommissionsPayableService(CommissionsPayableRepositoryMock.Object, CommissionsPayableDetailServiceMock.Object);
        }
    }

    public class CommissionsPayableServiceTests
    {
        [Fact]
        public async Task GetCommissionByPoolDetailIdAsync_Should_ReturnMappedResponse()
        {
            var poolDetailId = Guid.NewGuid();
            var commissions = new List<CommissionsPayable>
            {
                new() { Id = Guid.NewGuid(), CommissionPayable = 20m, IsActive = true, PoolDetail = new PoolDetail { PoolId = Guid.NewGuid() }, Order = new Order { Subtotal = 50m, Patient = new Patient { FirstName = "John", LastName = "Doe" }, Pharmacy = new Pharmacy { Name = "Pharmacy" } } }
            };
            var repo = new Mock<ICommissionsPayableRepository>();
            repo.Setup(r => r.FindWithIncludeAsync(
            It.IsAny<List<Expression<Func<CommissionsPayable, bool>>>>(),
            It.IsAny<string[]>(),
            It.IsAny<bool>()))
            .ReturnsAsync(commissions);

            var service = new CommissionsPayableServiceBuilder()
                .SetParameter(repo)
                .Build();

            var result = await service.GetCommissionByPoolDetailIdAsync(poolDetailId);

            result.Should().HaveCount(1);
            result[0].CommissionsPayableAmount.Should().Be(20m);
        }

        [Fact]
        public async Task GetCommissionByIdAsync_Should_ReturnDetailResponse()
        {
            var commission = new CommissionsPayable
            {
                Order = new Order
                {
                    Patient = new Patient { FirstName = "Jane", LastName = "Smith" },
                    Pharmacy = new Pharmacy { Name = "Main" },
                    Subtotal = 75m,
                    OrderDetails = new List<OrderDetail>()
                },
                PoolDetail = new PoolDetail { Pool = new Pool { FromDate = DateTime.UtcNow, ToDate = DateTime.UtcNow, Week = 1 } },
                CommissionPayable = 15m,
                CommissionBaseAmount = 60m,
                CTC = 10m,
                EntryType = CommissionEntryTypeEnum.Generated
            };

            var repo = new Mock<ICommissionsPayableRepository>();
            repo.Setup(r => r.GetWithIncludeAsync(It.IsAny<Guid>(), It.IsAny<string[]>()))
                .ReturnsAsync(commission);

            var service = new CommissionsPayableServiceBuilder()
                .SetParameter(repo)
                .Build();

            var result = await service.GetCommissionByIdAsync(Guid.NewGuid());

            result.Should().NotBeNull();
            result!.CommissionPayable.Should().Be(15m);
            result.CommissionAppliedTotal.Should().Be(60m);
        }

        [Fact]
        public async Task UpdateStatusCommissionPaybale_Should_ReturnMessage_When_NotFound()
        {
            var repo = new Mock<ICommissionsPayableRepository>();
            repo.Setup(r => r.FindWithIncludeAsync(
             It.IsAny<List<Expression<Func<CommissionsPayable, bool>>>>(),
             It.IsAny<string[]>(),
             It.IsAny<bool>()))
         .ReturnsAsync(new List<CommissionsPayable>());

            var service = new CommissionsPayableServiceBuilder()
                .SetParameter(repo)
                .Build();

            var result = await service.UpdateStatusCommissionPaybale(Guid.NewGuid(), 1);

            result.Message.Should().Be("No active commissions found for this order.");
            repo.Verify(r => r.BulkUpdateAsync(It.IsAny<List<CommissionsPayable>>()), Times.Never);
        }

        [Fact]
        public async Task UpdateStatusCommissionPaybale_Should_DeactivateCommissionsAndDetails()
        {
            var detail = new CommissionsPayablesDetail { IsActive = true };
            var commission = new CommissionsPayable
            {
                IsActive = true,
                OrderId = Guid.NewGuid(),
                CommissionsPayablesDetails = new List<CommissionsPayablesDetail> { detail }
            };

            var repo = new Mock<ICommissionsPayableRepository>();
            repo.Setup(r => r.FindWithIncludeAsync(
            It.IsAny<List<Expression<Func<CommissionsPayable, bool>>>>(),
            It.IsAny<string[]>(),
            It.IsAny<bool>()))
            .ReturnsAsync(new List<CommissionsPayable> { commission });

            var service = new CommissionsPayableServiceBuilder()
                .SetParameter(repo)
                .Build();

            var response = await service.UpdateStatusCommissionPaybale(Guid.NewGuid(), 7);

            commission.IsActive.Should().BeFalse();
            detail.IsActive.Should().BeFalse();
            repo.Verify(r => r.BulkUpdateAsync(It.Is<List<CommissionsPayable>>(c => c.All(x => !x.IsActive))), Times.Once);
            repo.Verify(r => r.SaveChangesAsync(), Times.Once);
            response.Message.Should().Be("Commission and related details deactivated successfully.");
        }

        [Fact]
        public async Task HasCommissionEntryAsync_Should_ReturnRepositoryResult()
        {
            var repo = new Mock<ICommissionsPayableRepository>();
            repo.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<CommissionsPayable, bool>>>()))
                .ReturnsAsync(true);

            var service = new CommissionsPayableServiceBuilder()
                .SetParameter(repo)
                .Build();

            var result = await service.HasCommissionEntryAsync(Guid.NewGuid(), CommissionEntryTypeEnum.Generated);

            result.Should().BeTrue();
        }
    }
}