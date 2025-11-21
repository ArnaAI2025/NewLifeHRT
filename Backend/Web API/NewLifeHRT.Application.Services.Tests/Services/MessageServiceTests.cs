using FluentAssertions;
using Moq;
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
    public class MessageServiceBuilder : ServiceBuilder<MessageService>
    {
        public override MessageService Build()
        {
            return new MessageService(MessageRepositoryMock.Object);
        }
    }

    public class MessageServiceTests
    {
        [Fact]
        public async Task CreateMessageAsync_Should_MarkUnreadInboundMessages_When_Outbound()
        {
            // Arrange
            var conversationId = Guid.NewGuid();
            var timestamp = DateTime.UtcNow;
            var request = new MessageRequestDto
            {
                ConversationId = conversationId,
                UserId = 10,
                TwilioId = "twilio",
                IsRead = false,
                Timestamp = timestamp,
                Direction = MessageDirectionEnum.Outbound.ToString(),
                IsSent = true
            };

            var existingMessages = new List<Message>
            {
                new() { Id = Guid.NewGuid(), ConversationId = conversationId, Timestamp = timestamp.AddMinutes(-5), Direction = MessageDirectionEnum.Inbound.ToString(), IsRead = false },
                new() { Id = Guid.NewGuid(), ConversationId = conversationId, Timestamp = timestamp.AddMinutes(-1), Direction = MessageDirectionEnum.Inbound.ToString(), IsRead = false }
            };

            var messageRepositoryMock = new Mock<IMessageRepository>();
            messageRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Message>()))
                .ReturnsAsync((Message m) => m);
            messageRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Message, bool>>>(), false))
                .ReturnsAsync(existingMessages);

            var messageService = new MessageServiceBuilder()
                .SetParameter(messageRepositoryMock)
                .Build();

            // Act
            var response = await messageService.CreateMessageAsync(request, 1, "creator");

            // Assert
            response.Message.Should().Contain("Message created successfully");
            messageRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Message>()), Times.Exactly(existingMessages.Count));
        }

        [Fact]
        public async Task UpdateIsReadAsync_Should_ReturnMessage_When_NoIdsProvided()
        {
            // Arrange
            var messageService = new MessageServiceBuilder().Build();

            // Act
            var response = await messageService.UpdateIsReadAsync(new BulkOperationRequestDto<Guid>(), 1);

            // Assert
            response.Should().NotBeNull();
            response.Message.Should().Be("No IDs provided for update.");
        }

        [Fact]
        public async Task UpdateIsReadAsync_Should_UpdateMessages()
        {
            // Arrange
            var messageIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var request = new BulkOperationRequestDto<Guid> { Ids = messageIds };

            var messages = messageIds.Select(id => new Message { Id = id }).ToList();

            var messageRepositoryMock = new Mock<IMessageRepository>();
            messageRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Message, bool>>>(), false))
                .ReturnsAsync(messages);

            var messageService = new MessageServiceBuilder()
                .SetParameter(messageRepositoryMock)
                .Build();

            // Act
            var response = await messageService.UpdateIsReadAsync(request, 5);

            // Assert
            response.Should().NotBeNull();
            response.SuccessCount.Should().Be(messages.Count);
            messageRepositoryMock.Verify(r => r.BulkUpdateAsync(It.Is<List<Message>>(m => m.Count() == messages.Count)), Times.Once);
        }
    }
}