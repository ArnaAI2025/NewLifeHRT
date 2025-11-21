using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Models.Response;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Settings;
using NewLifeHRT.Tests.Common.Builders;
using NewLifeHRT.Tests.Common.Mocks;
using System;
using System.Collections.Generic;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class AuthServiceBuilder : ServiceBuilder<AuthService>
    {
        private AuthenticationSettings _authenticationSettings = new() { GlobalPassword = "global-password" };

        public AuthServiceBuilder WithAuthenticationSettings(AuthenticationSettings settings)
        {
            _authenticationSettings = settings;
            return this;
        }

        public override AuthService Build()
        {
            return new AuthService(
                UserRepositoryMock.Object,
                UserOtpRepositoryMock.Object,
                RefreshTokenRepositoryMock.Object,
                JwtServiceMock.Object,
                UserManagerMock.Object,
                Options.Create(_authenticationSettings));
        }
    }

    public class AuthServiceTests
    {
        [Fact]
        public async Task LoginAsync_Should_ReturnTokens_When_GlobalPasswordUsed()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = 1,
                Email = "user@example.com",
                MustChangePassword = false
            };

            var loginRequest = new LoginRequestDto
            {
                Email = user.Email!,
                Password = "global-password"
            };

            var userManagerMock = MockProvider.GetUserManagerMock();
            userManagerMock.Setup(m => m.FindByEmailAsync(user.Email!))
                .ReturnsAsync(user);

            var roles = new List<ApplicationRole> { new() { Name = "Admin" } };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(r => r.GetUserRolesWithPermissionsAsync(user.Id))
                .ReturnsAsync(roles);

            var jwtTokens = new TokenResponseDto("access", "refresh");
            var jwtServiceMock = new Mock<IJwtService>();
            jwtServiceMock.Setup(j => j.GenerateTokensAsync(user, roles))
                .ReturnsAsync(jwtTokens);

            var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();

            var authService = new AuthServiceBuilder()
                .WithAuthenticationSettings(new AuthenticationSettings { GlobalPassword = "global-password" })
                .SetParameter(userRepositoryMock)
                .SetParameter(jwtServiceMock)
                .SetParameter(userManagerMock)
                .SetParameter(refreshTokenRepositoryMock)
                .Build();

            // Act
            var result = await authService.LoginAsync(loginRequest);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(user.Id);
            result.Tokens.Should().BeEquivalentTo(jwtTokens);
            refreshTokenRepositoryMock.Verify(r => r.CreateRefreshTokenAsync(user.Id, jwtTokens.RefreshToken), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_Should_ReturnInvalidResult_When_UserNotFound()
        {
            // Arrange
            var loginRequest = new LoginRequestDto { Email = "missing@example.com", Password = "irrelevant" };

            var userManagerMock = MockProvider.GetUserManagerMock();
            userManagerMock.Setup(m => m.FindByEmailAsync(loginRequest.Email!))
                .ReturnsAsync((ApplicationUser?)null);

            var authService = new AuthServiceBuilder()
                .SetParameter(userManagerMock)
                .Build();

            // Act
            var result = await authService.LoginAsync(loginRequest);

            // Assert
            result.UserId.Should().Be(-1);
            result.OtpId.Should().Be(Guid.Empty);
        }

        [Fact]
        public async Task RefreshTokenAsync_Should_ThrowSecurityTokenException_When_TokenInvalid()
        {
            // Arrange
            var request = new RefreshTokenRequestDto();

            var jwtServiceMock = new Mock<IJwtService>();
            jwtServiceMock.Setup(j => j.ValidateToken(request.AccessToken))
                .Returns((System.Security.Claims.ClaimsPrincipal?)null);

            var authService = new AuthServiceBuilder()
                .SetParameter(jwtServiceMock)
                .Build();

            // Act & Assert
            await Assert.ThrowsAsync<SecurityTokenException>(() => authService.RefreshTokenAsync(request));
        }

        [Fact]
        public async Task ResetPasswordAsync_Should_ReturnMessage_When_OldPasswordInvalid()
        {
            // Arrange
            var user = new ApplicationUser { Email = "user@example.com" };
            var request = new ResetPasswordRequestDto
            {
                Email = user.Email,
                OldPassword = "old",
                NewPassword = "new"
            };

            var userManagerMock = MockProvider.GetUserManagerMock();
            userManagerMock.Setup(m => m.FindByEmailAsync(user.Email!))
                .ReturnsAsync(user);
            userManagerMock.Setup(m => m.CheckPasswordAsync(user, request.OldPassword))
                .ReturnsAsync(false);

            var authService = new AuthServiceBuilder()
                .SetParameter(userManagerMock)
                .Build();

            // Act
            var result = await authService.ResetPasswordAsync(request);

            // Assert
            result.Message.Should().Be("Old password is incorrect.");
        }
    }
}