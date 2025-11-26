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
    public class ClinicServiceServiceBuilder : ServiceBuilder<ClinicServiceService>
    {
        public override ClinicServiceService Build()
        {
            return new ClinicServiceService(ClinicServiceRepositoryMock.Object);
        }
    }

    public class ClinicServiceServiceTests
    {
        [Fact]
        public async Task GetAllServiceByTypeAsync_Should_FilterByType_And_MapResults()
        {
            var services = new List<Service>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ServiceName = "Consultation",
                    DisplayName = "Consultation",
                    ServiceType = "Telehealth"
                }
            };

            var repositoryMock = new Mock<IClinicServiceRepository>();
            repositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Service, bool>>>(), It.IsAny<bool>())).ReturnsAsync(services);

            var service = new ClinicServiceServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var result = await service.GetAllServiceByTypeAsync("Telehealth");

            result.Should().ContainSingle().And.Subject.First().ServiceName.Should().Be("Consultation");
            repositoryMock.Verify(r => r.FindAsync(It.Is<Expression<Func<Service, bool>>>(expr => expr.Compile()(services[0])), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task GetAllAppointmentServicesAsync_Should_ReturnAppointmentDto()
        {
            var user = new ApplicationUser
            {
                Id = 7,
                FirstName = "Jane",
                LastName = "Doe",
                Timezone = new Timezone { Abbreviation = "PST" }
            };

            var services = new List<Service>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ServiceName = "Follow up",
                    DisplayName = "Follow up",
                    MaxDuration = TimeSpan.FromMinutes(45),
                    UserServices = new List<UserServiceLink>
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            User = user
                        }
                    }
                }
            };

            var repositoryMock = new Mock<IClinicServiceRepository>();
            repositoryMock.Setup(r => r.FindWithIncludeAsync(It.IsAny<List<Expression<Func<Service, bool>>>>(), It.IsAny<string[]>(), It.IsAny<bool>())).ReturnsAsync(services);

            var service = new ClinicServiceServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var result = await service.GetAllAppointmentServicesAsync();

            var appointment = result.Should().ContainSingle().Subject;
            appointment.MaxDuration.Should().Be("45 minutes");
            appointment.Users.Should().ContainSingle(u => u.UserId == 7 && u.TimezoneAbbreviation == "PST");
            repositoryMock.Verify(r => r.FindWithIncludeAsync(It.Is<List<Expression<Func<Service, bool>>>>(l => l.Count == 0), It.Is<string[]>(includes => includes.Contains("UserServices.User") && includes.Contains("UserServices.User.Timezone")), It.IsAny<bool>()), Times.Once);
        }
    }
}