using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using NewLifeHRT.Application.Services.Models.Request;
using NewLifeHRT.Application.Services.Services;
using NewLifeHRT.Domain.Entities;
using NewLifeHRT.Domain.Interfaces.Repositories;
using NewLifeHRT.Infrastructure.Data;
using NewLifeHRT.Tests.Common.Builders;
using NewLifeHRT.Tests.Common.Builders.Data;
using NewLifeHRT.Tests.Common.Fixtures;
using NewLifeHRT.Tests.Common.Mocks;
using NewLifeHRT.Tests.Common.Tests;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace NewLifeHRT.Application.Services.Tests.Services
{
    public class UserServiceBuilder : ServiceBuilder<UserService>
    {
        public override UserService Build()
        {
            return new UserService(
                UserRepositoryMock.Object,
                AddressRepositoryMock.Object,
                PasswordHasherMock.Object,
                UserManagerMock.Object,
                UserServiceLinkRepositoryMock.Object,
                LicenseInformationServiceMock.Object,
                BlobServiceMock.Object, null,
                UserSignatureRepositoryMock.Object,
                RoleManagerMock.Object
              );
        }
    }
    public class UserServiceTests : BaseServiceTest, IClassFixture<DatabaseFixture>
    {
        private readonly UserBuilder _userBuilder;
        public UserServiceTests(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
            _userBuilder = new UserBuilder(_fixture);
        }

        [Fact]
        public async Task CreateAsync_Should_Create_UserSuccessfully()
        {
            // Arrange
            var createUserRequestDto = new CreateUserRequestDto
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                Password = "Password123!",
                FirstName = "Test",
                LastName = "User",
                PhoneNumber = "1234567890",
                RoleIds = new List<int> { 1 }
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.ExistAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var userManagerMock = MockProvider.GetUserManagerMock();
            userManagerMock.Setup(manager => manager.CreateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var passwordHasherMock = new Mock<IPasswordHasher<ApplicationUser>>();
            passwordHasherMock.Setup(hasher => hasher.HashPassword(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Returns("hashedpassword");

            var userService = new UserServiceBuilder()
                .SetParameter(userRepositoryMock)
                .SetParameter(userManagerMock)
                .SetParameter(passwordHasherMock)
                .Build();

            // Act
            var result = await userService.CreateAsync(createUserRequestDto, 1);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("User created successfully");
            userRepositoryMock.Verify(repo => repo.ExistAsync(createUserRequestDto.UserName, createUserRequestDto.Email), Times.Once);
            userManagerMock.Verify(manager => manager.CreateAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }
        [Fact]
        public async Task CreateAsync_Should_ThrowException_When_UserExists()
        {
            // Arrange
            var createUserRequestDto = new CreateUserRequestDto
            {
                RoleIds = new List<int> { 1 }
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.ExistAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var userService = new UserServiceBuilder()
                .SetParameter(userRepositoryMock)
                .Build();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => userService.CreateAsync(createUserRequestDto, 1));
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_AllUsers()
        {
            // Arrange
            var users = new List<ApplicationUser>
            {
                _userBuilder.WithId(1).Build(),
                _userBuilder.WithId(2).Build()
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.FindWithIncludeAsync(It.IsAny<List<System.Linq.Expressions.Expression<System.Func<ApplicationUser, bool>>>>(), It.IsAny<string[]>(), It.IsAny<bool>()))
                .ReturnsAsync(users);

            var userService = new UserServiceBuilder()
                .SetParameter(userRepositoryMock)
                .Build();

            // Act
            var result = await userService.GetAllAsync(null);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_UsersByRole()
        {
            // Arrange
            var users = new List<ApplicationUser>
            {
                _userBuilder.WithId(1).Build(),
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.FindWithIncludeAsync(It.IsAny<List<System.Linq.Expressions.Expression<System.Func<ApplicationUser, bool>>>>(), It.IsAny<string[]>(), It.IsAny<bool>()))
                .ReturnsAsync(users);

            var userService = new UserServiceBuilder()
               .SetParameter(userRepositoryMock)
               .Build();

            // Act
            var result = await userService.GetAllAsync(new[] { 1 });

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
        }

        [Fact]
        public async Task GetAllActiveUsersAsync_Should_Return_ActiveUsers()
        {
            // Arrange
            var users = new List<ApplicationUser>
            {
                _userBuilder.WithId(1).Build(),
                _userBuilder.WithId(2).Build()
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ApplicationUser, bool>>>(), It.IsAny<bool>()))
                .ReturnsAsync(users);

            var userService = new UserServiceBuilder()
                .SetParameter(userRepositoryMock)
                .Build();

            // Act
            var result = await userService.GetAllActiveUsersAsync(new[] { 1 });

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnUser_When_UserExists()
        {
            // Arrange
            var user = _userBuilder.WithId(1).Build();

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.GetWithIncludeAsync(1, It.IsAny<string[]>()))
                .ReturnsAsync(user);

            var userService = new UserServiceBuilder()
               .SetParameter(userRepositoryMock)
               .Build();

            // Act
            var result = await userService.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnNull_When_UserDoesNotExist()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.GetWithIncludeAsync(1, It.IsAny<string[]>()))
                .ReturnsAsync((ApplicationUser)null);

            var userService = new UserServiceBuilder()
               .SetParameter(userRepositoryMock)
               .Build();

            // Act
            var result = await userService.GetByIdAsync(1);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task PermanentDeleteAsync_Should_Delete_UserSuccessfully()
        {
            // Arrange
            var user = _userBuilder.WithId(1).Build();

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(user);

            var userService = new UserServiceBuilder()
               .SetParameter(userRepositoryMock)
               .Build();

            // Act
            var result = await userService.PermanentDeleteAsync(1, 1);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("User permanently deleted");
            userRepositoryMock.Verify(repo => repo.DeleteAsync(user), Times.Once);
        }

        [Fact]
        public async Task PermanentDeleteAsync_Should_ThrowException_When_UserNotFound()
        {
            // Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync((ApplicationUser)null);

            var userService = new UserServiceBuilder()
               .SetParameter(userRepositoryMock)
               .Build();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => userService.PermanentDeleteAsync(1, 1));
        }

        [Fact]
        public async Task PermanentDeleteAsync_Should_ReturnErrorMessage_When_DeleteFails()
        {
            // Arrange
            var user = _userBuilder.WithId(1).Build();

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(user);
            userRepositoryMock.Setup(repo => repo.DeleteAsync(user))
                .ThrowsAsync(new Exception());

            var userService = new UserServiceBuilder()
                .SetParameter(userRepositoryMock)
                .Build();

            // Act
            var result = await userService.PermanentDeleteAsync(1, 1);

            // Assert
            result.Message.Should().Be("failed to delete user, user linked to the patients");
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_UserSuccessfully()
        {
            // Arrange
            var updateUserRequestDto = new UpdateUserRequestDto
            {
                FirstName = "Updated",
                LastName = "User",
                Email = "updated@example.com",
                PhoneNumber = "0987654321",
                RoleIds = new List<int> { 2 }
            };

            var user = _userBuilder.WithId(1).Build();

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(user);

            var userManagerMock = MockProvider.GetUserManagerMock();
            userManagerMock.Setup(manager => manager.UpdateAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var userService = new UserServiceBuilder()
               .SetParameter(userRepositoryMock)
               .SetParameter(userManagerMock)
               .Build();

            // Act
            var result = await userService.UpdateAsync(1, updateUserRequestDto, 1);

            // Assert
            result.Should().NotBeNull();
            result.Message.Should().Be("User updated successfully");
            userManagerMock.Verify(manager => manager.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Should_ThrowException_When_UserNotFound()
        {
            // Arrange
            var updateUserRequestDto = new UpdateUserRequestDto();

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync((ApplicationUser)null);

            var userService = new UserServiceBuilder()
                .SetParameter(userRepositoryMock)
                .Build();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => userService.UpdateAsync(1, updateUserRequestDto, 1));
        }

        [Fact]
        public async Task BulkToggleUserStatusAsync_Should_ToggleUserStatusSuccessfully()
        {
            // Arrange
            var userIds = new List<int> { 1, 2 };
            var users = new List<ApplicationUser>
            {
                _userBuilder.WithId(1).Build(),
                _userBuilder.WithId(2).Build()
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(users.FirstOrDefault(u => u.Id == id)));

            var userService = new UserServiceBuilder()
                .SetParameter(userRepositoryMock)
                .Build();

            // Act
            var result = await userService.BulkToggleUserStatusAsync(userIds, 1, true);

            // Assert
            result.Should().NotBeNull();
            result.SuccessCount.Should().Be(2);
            result.FailedCount.Should().Be(0);
        }

        [Fact]
        public async Task BulkToggleUserStatusAsync_Should_HandleFailedUpdates()
        {
            // Arrange
            var userIds = new List<int> { 1, 3 };
            var users = new List<ApplicationUser>
            {
                _userBuilder.WithId(1).Build(),
                _userBuilder.WithId(2).Build()
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .Returns<int>(id => Task.FromResult(users.FirstOrDefault(u => u.Id == id)));
            var clinicDbContextMock = new Mock<ClinicDbContext>();

            var userService = new UserServiceBuilder()
               .SetParameter(userRepositoryMock)
               .SetParameter(clinicDbContextMock)
               .Build();

            // Act
            var result = await userService.BulkToggleUserStatusAsync(userIds, 1, true);

            // Assert
            result.Should().NotBeNull();
            result.SuccessCount.Should().Be(1);
            result.FailedCount.Should().Be(1);
            result.FailedIds.Should().Contain("3");
        }
    }
}
