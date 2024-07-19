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
            var user = new User { Id = 1, Username = "testuser", Email = "test@example.com" };
            _mockUserRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task UpdateUserAsync_ValidUpdate_ReturnsUpdatedUserModel()
        {
            // Arrange
            var updateModel = new UserUpdateModel { Id = 1, Username = "updateduser", Email = "updated@example.com" };
            var existingUser = new User { Id = 1, Username = "testuser", Email = "test@example.com" };
            _mockUserRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingUser);
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(new User { Id = 1, Username = "updateduser", Email = "updated@example.com" });

            // Act
            var result = await _userService.UpdateUserAsync(updateModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateModel.Username, result.Username);
            Assert.Equal(updateModel.Email, result.Email);
        }

        [Fact]
        public async Task DeleteUserAsync_ExistingUser_ReturnsTrue()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testuser", Email = "test@example.com" };
            _mockUserRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _userService.DeleteUserAsync(1);

            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }
    }
}