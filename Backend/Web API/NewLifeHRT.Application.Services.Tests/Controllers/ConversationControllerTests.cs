using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class ConversationControllerTests
    {
        private readonly Mock<IConversationService> _conversationServiceMock = new();
        private readonly Mock<IMessageService> _messageServiceMock = new();
        private readonly Mock<ILogger<ConversationController>> _loggerMock = new();

        private ConversationController CreateController(int? userId = null)
        {
            var controller = new ConversationController(_conversationServiceMock.Object, _loggerMock.Object, _messageServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = userId.HasValue
                            ? new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()) }))
                            : new ClaimsPrincipal(new ClaimsIdentity())
                    }
                }
            };

            return controller;
        }

        [Fact]
        public async Task GetPatientConversation_Should_ReturnUnauthorized_When_NoUser()
        {
            // Arrange
            var controller = CreateController();

            // Act
            var result = await controller.GetPatientConversation(Guid.NewGuid());

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task ReceiveSms_Should_ReturnXmlContent()
        {
            // Arrange
            var request = new SmsRequestDto();
            var responseXml = "<Response></Response>";
            _conversationServiceMock.Setup(s => s.ProcessIncomingSmsAsync(request)).ReturnsAsync(responseXml);

            var controller = CreateController(1);

            // Act
            var result = await controller.ReceiveSms(request);

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);
            contentResult.ContentType.Should().Be("application/xml");
            contentResult.Content.Should().Be(responseXml);
        }

        [Fact]
        public async Task MarkMessagesAsRead_Should_ReturnOk_When_Authorized()
        {
            // Arrange
            var request = new BulkOperationRequestDto<Guid>();
            var response = new BulkOperationResponseDto();
            _messageServiceMock.Setup(m => m.UpdateIsReadAsync(request, 5)).ReturnsAsync(response);

            var controller = CreateController(5);

            // Act
            var result = await controller.MarkMessagesAsRead(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().Be(response);
        }

        [Fact]
        public async Task Create_Should_ReturnBadRequest_When_RequestNull()
        {
            var controller = CreateController(4);

            var result = await controller.Create(null!);

            result.Should().BeOfType<BadRequestObjectResult>();
            _conversationServiceMock.Verify(s => s.CreateConversation(It.IsAny<ConversationRequestDto>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetLeadConversation_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = CreateController();

            var result = await controller.GetLeadConversation(Guid.NewGuid());

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Create_Should_ReturnOk_When_ServiceSucceeds()
        {
            var dto = new ConversationRequestDto();
            var expected = new CommonOperationResponseDto<Guid> { Id = Guid.NewGuid() };
            _conversationServiceMock.Setup(s => s.CreateConversation(dto, 9))
                .ReturnsAsync(expected);

            var controller = CreateController(9);

            var result = await controller.Create(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().Be(expected);
        }
    }
}