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
    public class BatchMessageControllerTests
    {
        private readonly Mock<IBatchMessageService> _batchMessageServiceMock = new();

        private BatchMessageController CreateController(int? userId = null)
        {
            var httpContext = new DefaultHttpContext
            {
                User = userId.HasValue
                    ? new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()) }))
                    : new ClaimsPrincipal(new ClaimsIdentity())
            };

            return new BatchMessageController(_batchMessageServiceMock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };
        }

        [Fact]
        public async Task GetAll_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = CreateController();

            var result = await controller.GetAll(Guid.NewGuid());

            result.Should().BeOfType<UnauthorizedObjectResult>();
            _batchMessageServiceMock.Verify(s => s.GetAllAsync(), Times.Never);
        }

        [Fact]
        public async Task GetById_Should_ReturnNotFound_When_BatchMessageMissing()
        {
            _batchMessageServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((BatchMessageResponseDto?)null);

            var controller = CreateController(3);

            var result = await controller.GetById(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_Should_ReturnBadRequest_When_RequestNull()
        {
            var controller = CreateController(7);

            var result = await controller.Create(null!);

            result.Should().BeOfType<BadRequestObjectResult>();
            _batchMessageServiceMock.Verify(s => s.CreateAsync(It.IsAny<BatchMessageRequestDto>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Update_Should_ReturnUnauthorized_When_UserMissing()
        {
            var controller = CreateController();

            var result = await controller.Update(Guid.NewGuid(), new BatchMessageRequestDto());

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task BulkDelete_Should_ReturnOk_WithServiceResponse()
        {
            var expectedResponse = new BulkOperationResponseDto { Message = "done" };
            _batchMessageServiceMock.Setup(s => s.BulkDeleteAsync(It.IsAny<IList<Guid>>()))
                .ReturnsAsync(expectedResponse);

            var controller = CreateController(10);
            var request = new BulkOperationRequestDto<Guid> { Ids = new List<Guid> { Guid.NewGuid() } };

            var result = await controller.BulkDelete(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().Be(expectedResponse);
        }
    }
}