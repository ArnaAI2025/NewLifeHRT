using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Enums;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Tests.Common.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class BatchMessageRecipientServiceBuilder : ServiceBuilder<BatchMessageRecipientService>
    {
        public override BatchMessageRecipientService Build()
        {
            return new BatchMessageRecipientService(BatchMessageRecipientRepositoryMock.Object);
        }
    }

    public class BatchMessageRecipientServiceTests
    {
        [Fact]
        public async Task CreateAsync_Should_Throw_When_NoRecipients()
        {
            var service = new BatchMessageRecipientServiceBuilder().Build();

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(Guid.NewGuid(), null!, 1));
        }

        [Fact]
        public async Task CreateAsync_Should_Handle_Success_And_Failure()
        {
            var recipients = new List<BatchMessageRecipientRequestDto>
            {
                new() { PatientId = Guid.NewGuid() },
                new() { PatientId = Guid.NewGuid() }
            };

            var repositoryMock = new Mock<IBatchMessageRecipientRepository>();
            repositoryMock.SetupSequence(r => r.AddAsync(It.IsAny<BatchMessageRecipient>()))
                .ReturnsAsync(new BatchMessageRecipient())
                .ThrowsAsync(new Exception("failure"));

            var service = new BatchMessageRecipientServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var response = await service.CreateAsync(Guid.NewGuid(), recipients, 2);

            response.SuccessCount.Should().Be(1);
            response.FailedCount.Should().Be(1);
            repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_When_NoRecipientsFound()
        {
            var repositoryMock = new Mock<IBatchMessageRecipientRepository>();
            repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<BatchMessageRecipient, bool>>>(), false))
                .ReturnsAsync(Enumerable.Empty<BatchMessageRecipient>());

            var service = new BatchMessageRecipientServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateAsync(Guid.NewGuid(), (int)StatusEnum.Approved, 3));
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_AllRecipients()
        {
            var recipients = new List<BatchMessageRecipient>
            {
                new() { Id = Guid.NewGuid() },
                new() { Id = Guid.NewGuid() }
            };

            var repositoryMock = new Mock<IBatchMessageRecipientRepository>();
            repositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<BatchMessageRecipient, bool>>>(), false))
                .ReturnsAsync(recipients);

            var service = new BatchMessageRecipientServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var response = await service.UpdateAsync(Guid.NewGuid(), (int)StatusEnum.Approved, 6);

            response.SuccessCount.Should().Be(recipients.Count);
            repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<BatchMessageRecipient>()), Times.Exactly(recipients.Count));
        }
    }
}