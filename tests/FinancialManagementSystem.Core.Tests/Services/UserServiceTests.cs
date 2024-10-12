using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using FinancialManagementSystem.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FinancialManagementSystem.Core.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _userService = new UserService(_mockUserRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetUserAsync_ExistingUser_ReturnsUserModel()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId, Username = "testuser", Email = "test@example.com" };
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task GetUserAsync_NonExistingUser_ReturnsNull()
        {
            // Arrange
            var userId = 999; // Assume this user does not exist
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUserAsync_ValidUser_ReturnsUpdatedUserModel()
        {
            // Arrange
            var updateModel = new UserUpdateModel { Id = 1, Username = "updateduser", Email = "updated@example.com" };
            var user = new User { Id = updateModel.Id, Username = "olduser", Email = "old@example.com" };
            _mockUserRepository.Setup(r => r.GetByIdAsync(updateModel.Id)).ReturnsAsync(user);
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(new User { Id = updateModel.Id, Username = updateModel.Username, Email = updateModel.Email });

            // Act
            var result = await _userService.UpdateUserAsync(updateModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateModel.Username, result.Username);
            Assert.Equal(updateModel.Email, result.Email);
        }

        [Fact]
        public async Task UpdateUserAsync_NonExistingUser_ReturnsNull()
        {
            // Arrange
            var updateModel = new UserUpdateModel { Id = 999, Username = "nonexisting", Email = "nonexisting@example.com" };
            _mockUserRepository.Setup(r => r.GetByIdAsync(updateModel.Id)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.UpdateUserAsync(updateModel);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteUserAsync_ExistingUser_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId, Username = "testuser", Email = "test@example.com" };
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockUserRepository.Setup(r => r.DeleteAsync(userId)).Returns(Task.CompletedTask);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteUserAsync_NonExistingUser_ReturnsFalse()
        {
            // Arrange
            var userId = 999; // Assume this user does not exist
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsListOfUserModels()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Username = "user1", Email = "user1@example.com" },
                new User { Id = 2, Username = "user2", Email = "user2@example.com" }
            };
            _mockUserRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, u => u.Username == "user1");
            Assert.Contains(result, u => u.Username == "user2");
        }
    }
}
