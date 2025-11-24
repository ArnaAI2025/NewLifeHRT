using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using System.Linq.Expressions;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class CommisionRateServiceBuilder : ServiceBuilder<CommisionRateService>
    {
        public override CommisionRateService Build()
        {
            return new CommisionRateService(CommisionRateRepositoryMock.Object);
        }
    }

    public class CommisionRateServiceTests
    {
        [Fact]
        public async Task CreateCommisionRateAsync_Should_AddEntityAndReturnResponse()
        {
            var repo = new Mock<ICommisionRateRepository>();
            var service = new CommisionRateServiceBuilder()
                .SetParameter(repo)
                .Build();

            var dto = new CommisionRateRequestDto
            {
                ProductId = Guid.NewGuid(),
                FromAmount = 10,
                ToAmount = 100,
                RatePercentage = 5
            };

            var response = await service.CreateCommisionRateAsync(dto, 5);

            response.Message.Should().Be("Commision Rate Created Successfully");
            repo.Verify(r => r.AddAsync(It.IsAny<CommisionRate>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCommisionRateAsync_Should_ThrowException_When_NotFound()
        {
            var repo = new Mock<ICommisionRateRepository>();
            repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((CommisionRate?)null);

            var service = new CommisionRateServiceBuilder()
                .SetParameter(repo)
                .Build();

            var action = () => service.UpdateCommisionRateAsync(Guid.NewGuid(), new CommisionRateRequestDto(), 1);

            await action.Should().ThrowAsync<Exception>().WithMessage("Commision rate not found");
        }

        [Fact]
        public async Task ActivateCommisionRateAsync_Should_UpdateEntities()
        {
            var commisionRates = new List<CommisionRate> { new(Guid.NewGuid(), 1, 2, 3, "1", DateTime.UtcNow) { IsActive = false } };
            var repo = new Mock<ICommisionRateRepository>();
            repo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<CommisionRate, bool>>>(), false)).ReturnsAsync(commisionRates);
            var service = new CommisionRateServiceBuilder().SetParameter(repo).Build();

            await service.ActivateCommisionRateAsync(new List<Guid> { commisionRates[0].Id }, 9);

            commisionRates[0].IsActive.Should().BeTrue();
            commisionRates[0].UpdatedBy.Should().Be("9");
            repo.Verify(r => r.BulkUpdateAsync(commisionRates), Times.Once);
        }
    }
}