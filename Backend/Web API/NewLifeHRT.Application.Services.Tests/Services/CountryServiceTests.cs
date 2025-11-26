using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class CountryServiceBuilder : ServiceBuilder<CountryService>
    {
        public override CountryService Build()
        {
            return new CountryService(CountryRepositoryMock.Object);
        }
    }

    public class CountryServiceTests
    {
        [Fact]
        public async Task GetAllAsync_Should_ReturnActiveCountries()
        {
            var countries = new List<Country>
    {
        new() { Id = 1, Name = "USA", IsActive = true },
        new() { Id = 2, Name = "Canada", IsActive = true }
    };

            var repositoryMock = new Mock<ICountryRepository>();

            repositoryMock
                .Setup(r => r.FindAsync(
                    It.IsAny<Expression<Func<Country, bool>>>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(countries);

            var service = new CountryServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var result = await service.GetAllAsync();

            result.Should().HaveCount(2)
                  .And.OnlyContain(c => countries.Any(country => country.Id == c.Id));

            repositoryMock.Verify(r => r.FindAsync(
                    It.IsAny<Expression<Func<Country, bool>>>(),
                    It.IsAny<bool>()),
                Times.Once);
        }
    }
}