using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Models.Request;
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
    public class LicenseInformationServiceBuilder : ServiceBuilder<LicenseInformationService>
    {
        public override LicenseInformationService Build()
        {
            return new LicenseInformationService(LicenseInformationRepositoryMock.Object);
        }
    }

    public class LicenseInformationServiceTests
    {
        [Fact]
        public async Task CreateLicenseInformationAsync_Should_SaveRecords()
        {
            var request = new[]
            {
                new LicenseInformationRequestDto { StateId = 5, Number = "123" },
                new LicenseInformationRequestDto { StateId = 6, Number = "456" }
            };

            var repositoryMock = new Mock<ILicenseInformationRepository>();

            var service = new LicenseInformationServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var response = await service.CreateLicenseInformationAsync(request, 42, 7);

            response.Message.Should().Be("Inserted Successfully");
            repositoryMock.Verify(r => r.AddRangeAsync(It.Is<List<LicenseInformation>>(list => list.Count == 2 && list.All(li => li.UserId == 42))), Times.Once);

            repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateLicenseInformationAsync_Should_RemoveAll_When_RequestEmpty()
        {
            var existing = new List<LicenseInformation>
            {
                new() { Id = Guid.NewGuid(), StateId = 1, UserId = 99 },
                new() { Id = Guid.NewGuid(), StateId = 2, UserId = 99 }
            };

            var repositoryMock = new Mock<ILicenseInformationRepository>();
            repositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<LicenseInformation, bool>>>(), It.IsAny<bool>())).ReturnsAsync(existing);

            var service = new LicenseInformationServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var response = await service.UpdateLicenseInformationAsync(Array.Empty<LicenseInformationRequestDto>(), 99, 10);

            response.Message.Should().Be("All license information removed successfully");
            repositoryMock.Verify(r => r.RemoveRangeAsync(existing), Times.Once);
            repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateLicenseInformationAsync_Should_AddAndUpdate_AndRemoveMissing()
        {
            var existing = new List<LicenseInformation>
            {
                new() { Id = Guid.NewGuid(), StateId = 1, Number = "111", UserId = 50 },
                new() { Id = Guid.NewGuid(), StateId = 2, Number = "222", UserId = 50 }
            };

            var request = new[]
            {
                new LicenseInformationRequestDto { StateId = 1, Number = "111-updated" },
                new LicenseInformationRequestDto { StateId = 3, Number = "333" }
            };

            var repositoryMock = new Mock<ILicenseInformationRepository>();
            repositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<LicenseInformation, bool>>>(), It.IsAny<bool>())).ReturnsAsync(existing);

            var service = new LicenseInformationServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var response = await service.UpdateLicenseInformationAsync(request, 50, 25);

            response.Message.Should().Be("Updated Successfully");
            existing.Should().Contain(e => e.StateId == 1 && e.Number == "111-updated");
            repositoryMock.Verify(r => r.AddRangeAsync(It.Is<List<LicenseInformation>>(list => list.Any(li => li.StateId == 3 && li.CreatedBy == "25"))), Times.Once);
            repositoryMock.Verify(r => r.RemoveRangeAsync(It.Is<IEnumerable<LicenseInformation>>(list => list.Any(li => li.StateId == 2))), Times.Once);
            repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}