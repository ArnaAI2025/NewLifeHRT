using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class StateServiceBuilder : ServiceBuilder<StateService>
    {
        public override StateService Build()
        {
            return new StateService(StateRepositoryMock.Object);
        }
    }

    public class StateServiceTests
    {
        [Fact]
        public async Task GetAllAsync_Should_ReturnStatesForCountry()
        {
            var states = new List<State>
            {
                new() { Id = 10, Name = "California", Abbreviation = "CA", CountryId = 1, IsActive = true  },
                new() { Id = 11, Name = "Nevada", Abbreviation = "NV", CountryId = 1, IsActive = true  }
            };

            var repositoryMock = new Mock<IStateRepository>();
            repositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<State, bool>>>(), It.IsAny<bool>())).ReturnsAsync(states);

            var service = new StateServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var result = await service.GetAllAsync(1);

            result.Should().HaveCount(2);
            repositoryMock.Verify(r => r.FindAsync(It.Is<Expression<Func<State, bool>>>(expr => expr.Compile()(states[0])), It.IsAny<bool>()), Times.Once);
        }
    }
}