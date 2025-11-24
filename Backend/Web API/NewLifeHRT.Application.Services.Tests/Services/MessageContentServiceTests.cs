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
using System.Threading.Tasks;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class MessageContentServiceBuilder : ServiceBuilder<MessageContentService>
    {
        public override MessageContentService Build()
        {
            return new MessageContentService(MessageContentRepositoryMock.Object);
        }
    }

    public class MessageContentServiceTests
    {
        [Fact]
        public async Task CreateMessageContentAsync_Should_Add_Content()
        {
            var repositoryMock = new Mock<IMessageContentRepository>();
            repositoryMock.Setup(r => r.AddAsync(It.IsAny<MessageContent>()))
                .ReturnsAsync((MessageContent m) =>
                {
                    m.Id = Guid.NewGuid();
                    return m;
                });

            var service = new MessageContentServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var response = await service.CreateMessageContentAsync(new MessageContentRequestDto
            {
                MessageId = Guid.NewGuid(),
                ContentType = "text",
                Content = "hello"
            }, "user");

            response.Id.Should().NotBeEmpty();
            repositoryMock.Verify(r => r.AddAsync(It.Is<MessageContent>(m => m.Content == "hello")), Times.Once);
        }

        [Fact]
        public async Task GetNonTextMessageContentsByPatientIdAsync_Should_Return_FilteredContents()
        {
            var patientId = Guid.NewGuid();
            var contents = new List<MessageContent>
            {
                new() { Id = Guid.NewGuid(), ContentType = "image/png", Message = new Message { Conversation = new Conversation { PatientId = patientId } } }
            };

            var repositoryMock = new Mock<IMessageContentRepository>();
            repositoryMock.Setup(r => r.FindWithIncludeAsync(It.IsAny<List<System.Linq.Expressions.Expression<Func<MessageContent, bool>>>>(), It.IsAny<string[]>(), true))
                .ReturnsAsync(contents);

            var service = new MessageContentServiceBuilder()
                .SetParameter(repositoryMock)
                .Build();

            var result = await service.GetNonTextMessageContentsByPatientIdAsync(patientId);

            result.Should().HaveCount(1);
            result.First().ContentType.Should().Be("image/png");
        }
    }
}