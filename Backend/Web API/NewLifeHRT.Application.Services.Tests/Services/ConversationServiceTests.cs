using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Application.Services.Services.Hubs;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.External.Interfaces;
using NewLifeHRT.Tests.Common.Builders;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class ConversationServiceBuilder : ServiceBuilder<ConversationService>
    {
        public override ConversationService Build()
        {
            return new ConversationService(
                ConversationRepositoryMock.Object,
                MessageContentServiceMock.Object,
                MessageServiceMock.Object,
                SmsServiceMock.Object,
                PatientServiceMock.Object,
                LeadServiceMock.Object,
                HubContextMock.Object,
                AudioConverterMock.Object,
                BlobServiceMock.Object);
        }
    }

    public class ConversationServiceTests
    {
        [Fact]
        public async Task CreateConversation_Should_Return_Error_When_MissingPhoneOrContent()
        {
            var service = new ConversationServiceBuilder().Build();
            var dto = new ConversationRequestDto
            {
                To = "",
                MessageContent = new MessageContentRequestDto { Content = "" },
                Message = new MessageRequestDto()
            };

            var response = await service.CreateConversation(dto, 1);

            response.Id.Should().Be(Guid.Empty);
            response.Message.Should().Contain("Phone number and message are required.");
        }

        [Fact]
        public async Task GetOrCreateConversationAsync_Should_Create_When_NotFound()
        {
            var repositoryMock = new Mock<IConversationRepository>();
            repositoryMock.Setup(r => r.GetSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Conversation, bool>>>(), null, false))
                .ReturnsAsync((Conversation?)null);

            var createdConversation = new Conversation { Id = Guid.NewGuid(), PatientId = Guid.NewGuid() };
            repositoryMock.Setup(r => r.AddAsync(It.IsAny<Conversation>())).ReturnsAsync(createdConversation);

            var service = new ConversationServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var result = await service.GetOrCreateConversationAsync(createdConversation.PatientId, null, 3, "111", "creator");

            result.Should().NotBeNull();
            repositoryMock.Verify(r => r.AddAsync(It.Is<Conversation>(c => c.PatientId == createdConversation.PatientId)), Times.Once);
            repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ProcessIncomingSmsAsync_Should_SaveMessage_AndNotifyClients()
        {
            var patientId = Guid.NewGuid();
            var conversationId = Guid.NewGuid();
            var messageId = Guid.NewGuid();

            var patientServiceMock = new Mock<IPatientService>();
            patientServiceMock.Setup(s => s.GetPatientByMobileNumber(It.IsAny<string>()))
                .ReturnsAsync(new Patient { Id = patientId, CounselorId = 12 });

            var conversationRepositoryMock = new Mock<IConversationRepository>();
            conversationRepositoryMock.Setup(r => r.GetSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Conversation, bool>>>(), null, false))
                .ReturnsAsync((Conversation?)null);
            conversationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Conversation>()))
                .ReturnsAsync(new Conversation { Id = conversationId, PatientId = patientId });

            var messageServiceMock = new Mock<IMessageService>();
            messageServiceMock.Setup(s => s.CreateMessageAsync(It.IsAny<MessageRequestDto>(), 0, It.IsAny<string>()))
                .ReturnsAsync(new CommonOperationResponseDto<Guid> { Id = messageId });

            var messageContentServiceMock = new Mock<IMessageContentService>();
            messageContentServiceMock.Setup(s => s.CreateMessageContentAsync(It.IsAny<MessageContentRequestDto>(), It.IsAny<string>()))
                .ReturnsAsync(new CommonOperationResponseDto<Guid>());

            var clientProxyMock = new Mock<IClientProxy>();
            clientProxyMock.Setup(c => c.SendCoreAsync("ReceiveMessage", It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var hubClientsMock = new Mock<IHubClients>();
            hubClientsMock.Setup(c => c.Group(patientId.ToString())).Returns(clientProxyMock.Object);

            var hubContextMock = new Mock<IHubContext<SmsHub>>();
            hubContextMock.Setup(h => h.Clients).Returns(hubClientsMock.Object);

            var service = new ConversationServiceBuilder()
                .SetParameter(patientServiceMock)
                .SetParameter(conversationRepositoryMock)
                .SetParameter(messageServiceMock)
                .SetParameter(messageContentServiceMock)
                .SetParameter(hubContextMock)
                .Build();

            var request = new SmsRequestDto
            {
                From = "+123456",
                SmsSid = "sid",
                Body = "hello"
            };

            var xml = await service.ProcessIncomingSmsAsync(request);

            xml.Should().Contain("Response");
            messageServiceMock.Verify(s => s.CreateMessageAsync(It.IsAny<MessageRequestDto>(), 0, It.IsAny<string>()), Times.Once);
            messageContentServiceMock.Verify(s => s.CreateMessageContentAsync(It.Is<MessageContentRequestDto>(m => m.Content == request.Body), It.IsAny<string>()), Times.Once);
            clientProxyMock.Verify(c => c.SendCoreAsync("ReceiveMessage", It.IsAny<object?[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}