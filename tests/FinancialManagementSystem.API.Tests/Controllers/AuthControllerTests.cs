using FinancialManagementSystem.API.Controllers;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using FinancialManagementSystem.Core.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FinancialManagementSystem.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "testuser", Password = "password" };
            _mockAuthService.Setup(s => s.LoginAsync(loginModel))
                .ReturnsAsync(new AuthResult { Succeeded = true, Token = "test_token" });

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "testuser", Password = "wrongpassword" };
            _mockAuthService.Setup(s => s.LoginAsync(loginModel))
                .ReturnsAsync(new AuthResult { Succeeded = false, ErrorMessage = "Invalid credentials" });

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid credentials", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Register_ValidModel_ReturnsOkResult()
        {
            // Arrange
            var registerModel = new RegisterModel { Username = "newuser", Email = "new@example.com", Password = "password" };
            _mockAuthService.Setup(s => s.RegisterAsync(registerModel))
                .ReturnsAsync(new AuthResult { Succeeded = true, Token = "new_token" });

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Register_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var registerModel = new RegisterModel { Username = "existinguser", Email = "existing@example.com", Password = "password" };
            _mockAuthService.Setup(s => s.RegisterAsync(registerModel))
                .ReturnsAsync(new AuthResult { Succeeded = false, ErrorMessage = "Username already exists" });

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username already exists", badRequestResult.Value);
        }
    }
}
