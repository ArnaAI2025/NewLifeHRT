using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class CommissionsPayableDetailServiceBuilder : ServiceBuilder<CommissionsPayableDetailService>
    {
        public override CommissionsPayableDetailService Build()
        {
            return new CommissionsPayableDetailService(CommissionsPayableDetailRepositoryMock.Object);
        }
    }

    public class CommissionsPayableDetailServiceTests
    {
        [Fact]
        public async Task InsertAsync_Should_ReturnMessage_When_InputNull()
        {
            var repo = new Mock<ICommissionsPayableDetailRepository>();
            var service = new CommissionsPayableDetailServiceBuilder()
                .SetParameter(repo)
                .Build();

            var response = await service.InsertAsync(null!);

            response.Message.Should().Be("Input cannot be null or empty.");
            repo.Verify(r => r.AddRangeAsync(It.IsAny<List<CommissionsPayablesDetail>>()), Times.Never);
        }

        [Fact]
        public async Task InsertAsync_Should_AddDetails_When_InputValid()
        {
            var details = new List<CommissionsPayablesDetail> { new() };
            var repo = new Mock<ICommissionsPayableDetailRepository>();

            var service = new CommissionsPayableDetailServiceBuilder()
                .SetParameter(repo)
                .Build();

            var response = await service.InsertAsync(details);

            response.Message.Should().Be("Commission Details added Successfully");
            repo.Verify(r => r.AddRangeAsync(details), Times.Once);
        }
    }
}