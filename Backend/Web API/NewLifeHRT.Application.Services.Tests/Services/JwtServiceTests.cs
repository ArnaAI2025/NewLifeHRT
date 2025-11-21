using FluentAssertions;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Infrastructure.Models.MultiTenancy;
using NewLifeHRT.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class JwtServiceTests
    {
        [Fact]
        public async Task GenerateTokensAsync_Should_ReturnTokens_WithClaims()
        {
            // Arrange
            var tenantInfo = new MultiTenantInfo
            {
                Id = Guid.NewGuid().ToString(),
                Identifier = "tenant-1",
                HostUrl = "https://tenant.example.com",
                JwtBearerAudience = "tenant-aud"
            };

            var tenantContext = new MultiTenantContext<MultiTenantInfo>
            {
                TenantInfo = tenantInfo
            };

            var tenantAccessorMock = new Mock<IMultiTenantContextAccessor<MultiTenantInfo>>();
            tenantAccessorMock.Setup(a => a.MultiTenantContext).Returns(tenantContext);

            var jwtSettings = new JwtSettings
            {
                Key = "98765432103216549870012345678901",
                ExpiryTime = 60
            };

            var config = new ConfigurationBuilder().Build();
            var jwtService = new JwtService(config, tenantAccessorMock.Object, Options.Create(jwtSettings));

            var user = new ApplicationUser { Id = 99, Email = "user@example.com", FirstName = "Test", LastName = "User" };
            var roles = new List<ApplicationRole>
            {
                new()
                {
                    Name = "Admin",
                    RolePermissions = new List<RolePermission>
                    {
                        new() { Permission = new Permission { PermissionName = "CanView" } }
                    }
                }
            };

            // Act
            var tokens = await jwtService.GenerateTokensAsync(user, roles);

            // Assert
            tokens.AccessToken.Should().NotBeNullOrWhiteSpace();
            tokens.RefreshToken.Should().NotBeNullOrWhiteSpace();

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(tokens.AccessToken);

            jwt.Claims.Should().Contain(c => c.Type == "email" && c.Value == user.Email);
            jwt.Claims.Should().Contain(c => c.Type == "tenant" && c.Value == tenantInfo.Identifier);
            jwt.Claims.Should().Contain(c => c.Type == "role" && c.Value == "Admin");
            jwt.Claims.Should().Contain(c => c.Type == "permission" && c.Value == "CanView");
        }

        [Fact]
        public async Task GenerateTokensAsync_Should_Throw_When_RolesMissing()
        {
            // Arrange
            var tenantAccessorMock = new Mock<IMultiTenantContextAccessor<MultiTenantInfo>>();
            tenantAccessorMock.Setup(a => a.MultiTenantContext).Returns(new MultiTenantContext<MultiTenantInfo>
            {
                TenantInfo = new MultiTenantInfo
                {
                    Identifier = "tenant",
                    HostUrl = "https://tenant",
                    JwtBearerAudience = "aud"
                }
            });

            var jwtSettings = new JwtSettings { Key = "key", ExpiryTime = 30 };
            var config = new ConfigurationBuilder().Build();
            var jwtService = new JwtService(config, tenantAccessorMock.Object, Options.Create(jwtSettings));

            var user = new ApplicationUser { Id = 1, Email = "user@example.com" };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => jwtService.GenerateTokensAsync(user, null!));
        }

        [Fact]
        public void ValidateToken_Should_ReturnNull_When_Invalid()
        {
            // Arrange
            var tenantAccessorMock = new Mock<IMultiTenantContextAccessor<MultiTenantInfo>>();
            var jwtSettings = new JwtSettings { Key = "key", ExpiryTime = 30 };
            var config = new ConfigurationBuilder().Build();
            var jwtService = new JwtService(config, tenantAccessorMock.Object, Options.Create(jwtSettings));

            // Act
            var principal = jwtService.ValidateToken("not-a-jwt");

            // Assert
            principal.Should().BeNull();
        }
    }
}