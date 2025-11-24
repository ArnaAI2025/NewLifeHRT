using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using System.Security.Claims;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class ReminderControllerTests
    {
        private readonly Mock<IReminderService> _reminderServiceMock = new();

        private ReminderController CreateController(int? userId = null)
        {
            var httpContext = new DefaultHttpContext
            {
                User = userId.HasValue
                    ? new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()) }))
                    : new ClaimsPrincipal(new ClaimsIdentity())
            };

            return new ReminderController(_reminderServiceMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };
        }

        [Fact]
        public async Task CreateReminder_Should_ReturnBadRequest_When_NoEntityProvided()
        {
            var controller = CreateController(5);

            var result = await controller.CreateReminder(new CreateReminderRequestDto());

            result.Should().BeOfType<BadRequestObjectResult>();
            _reminderServiceMock.Verify(s => s.CreateReminderAsync(It.IsAny<CreateReminderRequestDto>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task CreateReminder_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = CreateController();

            var result = await controller.CreateReminder(new CreateReminderRequestDto { PatientId = Guid.NewGuid() });

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task CreateReminder_Should_ReturnOk_When_RequestValid()
        {
            var dto = new CreateReminderRequestDto { PatientId = Guid.NewGuid(), ReminderTypeId = 1, ReminderDateTime = DateTime.UtcNow };
            var response = new CommonOperationResponseDto<Guid> { Id = Guid.NewGuid() };
            _reminderServiceMock.Setup(s => s.CreateReminderAsync(dto, 8)).ReturnsAsync(response);

            var controller = CreateController(8);

            var result = await controller.CreateReminder(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().Be(response);
        }

        [Fact]
        public async Task MarkReminderAsCompleted_Should_ReturnNotFound_When_ResponseEmpty()
        {
            _reminderServiceMock.Setup(s => s.MarkReminderAsCompletedAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new CommonOperationResponseDto<Guid> { Id = Guid.Empty, Message = "missing" });

            var controller = CreateController(1);

            var result = await controller.MarkReminderAsCompleted(Guid.NewGuid());

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetAllActiveRemindersForPatients_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = CreateController();

            var result = await controller.GetAllActiveRemindersForPatients();

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}