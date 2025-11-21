using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MultiTenantTest.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock = new();

        private static UserController CreateController(Mock<IUserService> userServiceMock, int? userId = null)
        {
            var controller = new UserController(userServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = userId.HasValue
                            ? new ClaimsPrincipal(new ClaimsIdentity(new[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString())
                            }))
                            : new ClaimsPrincipal(new ClaimsIdentity())
                    }
                }
            };

            return controller;
        }

        [Fact]
        public async Task GetById_Should_ReturnNotFound_When_UserMissing()
        {
            // Arrange
            _userServiceMock.Setup(s => s.GetByIdAsync(5)).ReturnsAsync((Application.Services.Models.Response.UserResponseDto?)null);
            var controller = CreateController(_userServiceMock);

            // Act
            var result = await controller.GetById(5);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_Should_ReturnUnauthorized_When_NoUserId()
        {
            // Arrange
            var controller = CreateController(_userServiceMock);

            // Act
            var result = await controller.Create(new CreateUserRequestDto());

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Create_Should_ReturnOk_WithServiceResponse()
        {
            // Arrange
            var response = new Application.Services.Models.Response.CommonOperationResponseDto<int> { Id = 2, Message = "created" };
            _userServiceMock.Setup(s => s.CreateAsync(It.IsAny<CreateUserRequestDto>(), 10)).ReturnsAsync(response);
            var controller = CreateController(_userServiceMock, 10);

            // Act
            var result = await controller.Create(new CreateUserRequestDto());

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(response);
        }

        [Fact]
        public async Task DeleteUsers_Should_ReturnBadRequest_When_IdsMissing()
        {
            // Arrange
            var controller = CreateController(_userServiceMock, 1);

            // Act
            var result = await controller.DeleteUsers(new BulkOperationRequestDto<int>());

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteUsers_Should_ReturnUnauthorized_When_UserNotAuthenticated()
        {
            // Arrange
            var request = new BulkOperationRequestDto<int> { Ids = new List<int> { 1 } };
            var controller = CreateController(_userServiceMock);

            // Act
            var result = await controller.DeleteUsers(request);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}