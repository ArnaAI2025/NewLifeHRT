using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using System.Linq.Expressions;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class PoolServiceBuilder : ServiceBuilder<PoolService>
    {
        public override PoolService Build()
        {
            return new PoolService(PoolRepositoryMock.Object);
        }
    }

    public class PoolServiceTests
    {
        [Fact]
        public async Task GetPoolInformationAsync_Should_ReturnNull_When_InvalidParameters()
        {
            var repo = new Mock<IPoolRepository>();
            var service = new PoolServiceBuilder()
                .SetParameter(repo)
                .Build();

            var result = await service.GetPoolInformationAsync(default, DateTime.UtcNow, 1);

            result.Should().BeNull();
            repo.Verify(r => r.FindWithIncludeAsync(It.IsAny<List<Expression<Func<Pool, bool>>>>(), It.IsAny<string[]>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task GetPoolInformationAsync_Should_ReturnPool_When_CounselorMatches()
        {
            var counselorId = 3;
            var pools = new List<Pool>
            {
                new()
                {
                    FromDate = DateTime.UtcNow.Date,
                    ToDate = DateTime.UtcNow.Date,
                    PoolDetails = new List<PoolDetail> { new() { CounselorId = counselorId } }
                }
            };

            var repo = new Mock<IPoolRepository>();
            repo.Setup(r => r.FindWithIncludeAsync(It.IsAny<List<Expression<Func<Pool, bool>>>>(), It.IsAny<string[]>(), It.IsAny<bool>())).ReturnsAsync(pools);

            var service = new PoolServiceBuilder()
                .SetParameter(repo)
                .Build();

            var result = await service.GetPoolInformationAsync(DateTime.UtcNow.Date, DateTime.UtcNow.Date, counselorId);

            result.Should().Be(pools[0]);
            repo.Verify(r => r.FindWithIncludeAsync(It.IsAny<List<Expression<Func<Pool, bool>>>>(), It.IsAny<string[]>(), It.IsAny<bool>()), Times.Once);
        }
    }
}