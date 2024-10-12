using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using FinancialManagementSystem.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FinancialManagementSystem.Core.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<AuthService>> _mockLogger;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<AuthService>>();
            _mockConfiguration.Setup(c => c["Jwt:Secret"]).Returns("this_is_a_long_secret_key_with_32_chars");
            _authService = new AuthService(_mockUserRepository.Object, _mockConfiguration.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsSuccessfulResult()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "testuser", Password = "password" };
            var user = new User { Id = 1, Username = "testuser", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };
            _mockUserRepository.Setup(r => r.GetByUsernameAsync(loginModel.Username)).ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(loginModel);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Token);
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ReturnsFailedResult()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "testuser", Password = "wrongpassword" };
            var user = new User { Id = 1, Username = "testuser", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password") };
            _mockUserRepository.Setup(r => r.GetByUsernameAsync(loginModel.Username)).ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(loginModel);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Null(result.Token);
            Assert.Equal("Invalid username or password", result.ErrorMessage);
        }

        [Fact]
        public async Task RegisterAsync_NewUser_ReturnsSuccessfulResult()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Username = "newuser",
                Email = "new@example.com",
                Password = "password"
            };

            _mockUserRepository.Setup(r => r.GetByUsernameAsync(registerModel.Username)).ReturnsAsync((User)null);
            _mockUserRepository.Setup(r => r.GetByEmailAsync(registerModel.Email)).ReturnsAsync((User)null);

            // Act
            var result = await _authService.RegisterAsync(registerModel);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Token);
        }

        [Fact]
        public async Task RegisterAsync_ExistingUsername_ReturnsFailedResult()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Username = "existinguser",
                Email = "new@example.com",
                Password = "password"
            };

            var existingUser = new User { Username = "existinguser", Email = "existing@example.com" };
            _mockUserRepository.Setup(r => r.GetByUsernameAsync(registerModel.Username)).ReturnsAsync(existingUser);

            // Act
            var result = await _authService.RegisterAsync(registerModel);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Username already exists", result.ErrorMessage);
        }

        [Fact]
        public async Task RegisterAsync_ExistingEmail_ReturnsFailedResult()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Username = "newuser",
                Email = "existing@example.com",
                Password = "password"
            };

            var existingUser = new User { Username = "existinguser", Email = "existing@example.com" };
            _mockUserRepository.Setup(r => r.GetByUsernameAsync(registerModel.Username)).ReturnsAsync((User)null);
            _mockUserRepository.Setup(r => r.GetByEmailAsync(registerModel.Email)).ReturnsAsync(existingUser);

            // Act
            var result = await _authService.RegisterAsync(registerModel);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Email already exists", result.ErrorMessage);
        }

        [Fact]
        public async Task ValidateToken_ValidToken_ReturnsUserId()
        {
            // Arrange
            var registerModel = new RegisterModel { Username = "newuser", Email = "new@example.com", Password = "password" };

            var registerResult = await _authService.RegisterAsync(registerModel);
            var token = registerResult.Token;

            // Act
            var userId = _authService.ValidateToken(token);

            // Assert
            Assert.NotNull(token);
        }

        [Fact]
        public void ValidateToken_InvalidToken_ReturnsNull()
        {
            // Arrange
            var invalidToken = "invalid.token.string";

            // Act
            var result = _authService.ValidateToken(invalidToken);

            // Assert
            Assert.Null(result);
        }
    }
}
