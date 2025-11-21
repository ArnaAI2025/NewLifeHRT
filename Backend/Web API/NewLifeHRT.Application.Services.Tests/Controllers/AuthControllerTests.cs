using FluentAssertions;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NewLifeHRT.API.Controllers.Controllers;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using System.Threading.Tasks;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock = new();
        private readonly Mock<IMultiTenantContextAccessor<MultiTenantInfo>> _tenantAccessorMock = new();

        [Fact]
        public async Task Login_Should_ReturnServiceResult()
        {
            // Arrange
            var request = new LoginRequestDto { Email = "user@example.com", Password = "password" };
            var expected = new LoginResponseDto(1, new TokenResponseDto("access", "refresh"));

            _authServiceMock.Setup(s => s.LoginAsync(request)).ReturnsAsync(expected);

            var controller = new AuthController(_authServiceMock.Object, _tenantAccessorMock.Object);

            // Act
            var result = await controller.Login(request);

            // Assert
            result.Value.Should().Be(expected);
            _authServiceMock.Verify(s => s.LoginAsync(request), Times.Once);
        }

        [Fact]
        public async Task ResetPassword_Should_ReturnOkResult()
        {
            // Arrange
            var request = new ResetPasswordRequestDto { Email = "user@example.com", OldPassword = "old", NewPassword = "new" };
            var expected = new CommonOperationResponseDto<int?> { Id = 1, Message = "Password changed successfully." };

            _authServiceMock.Setup(s => s.ResetPasswordAsync(request))
                .ReturnsAsync(expected);

            var controller = new AuthController(_authServiceMock.Object, _tenantAccessorMock.Object);

            // Act
            var result = await controller.ResetPassword(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().Be(expected);
        }
    }
}