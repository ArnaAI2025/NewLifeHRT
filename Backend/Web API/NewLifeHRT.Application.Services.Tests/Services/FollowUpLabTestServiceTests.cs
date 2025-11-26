using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using System.Collections.Generic;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class FollowUpLabTestServiceBuilder : ServiceBuilder<FollowUpLabTestService>
    {
        public override FollowUpLabTestService Build()
        {
            return new FollowUpLabTestService(FollowUpLabTestRepositoryMock.Object);
        }
    }

    public class FollowUpLabTestServiceTests
    {
        [Fact]
        public async Task GetAllFollowUpTestAsync_Should_ReturnDropDownList()
        {
            var followUpTests = new List<FollowUpLabTest>
            {
                new() { Id = 1, Duration = "2 weeks" },
                new() { Id = 2, Duration = "1 month" }
            };

            var repositoryMock = new Mock<IFollowUpLabTestRepository>();
            repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(followUpTests);

            var service = new FollowUpLabTestServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var result = await service.GetAllFollowUpTestAsync();

            result.Should().HaveCount(2)
                .And.Contain(dto => dto.Value == "2 weeks")
                .And.Contain(dto => dto.Value == "1 month");
            repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
}