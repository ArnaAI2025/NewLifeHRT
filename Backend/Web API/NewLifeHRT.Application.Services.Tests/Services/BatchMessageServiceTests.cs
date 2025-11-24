using FluentAssertions;
using Moq;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
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
    public class BatchMessageServiceBuilder : ServiceBuilder<BatchMessageService>
    {
        public override BatchMessageService Build()
        {
            return new BatchMessageService(BatchMessageRepositoryMock.Object, BatchMessageRecipientServiceMock.Object);
        }
    }

    public class BatchMessageServiceTests
    {
        [Fact]
        public async Task GetAllAsync_Should_ReturnNull_When_NoMessagesFound()
        {
            var repositoryMock = new Mock<IBatchMessageRepository>();
            repositoryMock.Setup(r => r.FindWithIncludeAsync(It.IsAny<List<System.Linq.Expressions.Expression<Func<BatchMessage, bool>>>>(), It.IsAny<string[]>(), false))
                .ReturnsAsync(new List<BatchMessage>());

            var service = new BatchMessageServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var result = await service.GetAllAsync();

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_Should_CreateBatchMessage_AndRecipients()
        {
            var request = new BatchMessageRequestDto
            {
                Subject = "subject",
                Message = "message",
                BatchMessageRecipients = new List<BatchMessageRecipientRequestDto> { new() { PatientId = Guid.NewGuid() } }
            };

            var created = new BatchMessage { Id = Guid.NewGuid() };
            var repositoryMock = new Mock<IBatchMessageRepository>();
            repositoryMock.Setup(r => r.AddAsync(It.IsAny<BatchMessage>()))
                .ReturnsAsync(created);

            var recipientServiceMock = new Mock<IBatchMessageRecipientService>();
            recipientServiceMock.Setup(s => s.CreateAsync(created.Id, request.BatchMessageRecipients, 4))
                .ReturnsAsync(new BulkOperationResponseDto());

            var service = new BatchMessageServiceBuilder()
                .SetParameter(repositoryMock)
                .SetParameter(recipientServiceMock)
                .Build();

            var result = await service.CreateAsync(request, 4);

            result.Id.Should().Be(created.Id);
            repositoryMock.Verify(r => r.AddAsync(It.Is<BatchMessage>(b => b.Subject == request.Subject)), Times.Once);
            recipientServiceMock.Verify(s => s.CreateAsync(created.Id, request.BatchMessageRecipients, 4), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_When_EntityNotFound()
        {
            var repositoryMock = new Mock<IBatchMessageRepository>();
            repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((BatchMessage?)null);

            var service = new BatchMessageServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateAsync(Guid.NewGuid(), new BatchMessageRequestDto(), (int)StatusEnum.InProgress, 1));
        }

        [Fact]
        public async Task BulkDeleteAsync_Should_ReturnMessage_When_NoIds()
        {
            var service = new BatchMessageServiceBuilder().Build();

            var response = await service.BulkDeleteAsync(new List<Guid>());

            response.Message.Should().Contain("No valid messages IDs provided.");
            response.SuccessCount.Should().Be(0);
        }
    }
}