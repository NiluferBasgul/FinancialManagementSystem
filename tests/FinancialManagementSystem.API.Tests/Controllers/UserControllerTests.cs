using FinancialManagementSystem.API.Controllers;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace FinancialManagementSystem.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UserController(_mockUserService.Object);
            SetupControllerContext();
        }

        private void SetupControllerContext()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetUser_ExistingUser_ReturnsOkResult()
        {
            // Arrange
            var userId = 1;
            var userModel = new UserModel { Id = userId, Username = "testuser", Email = "test@example.com" };
            _mockUserService.Setup(s => s.GetUserAsync(userId)).ReturnsAsync(userModel);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserModel>(okResult.Value);
            Assert.Equal(userId, returnValue.Id);
        }

        [Fact]
        public async Task GetUser_NonExistingUser_ReturnsNotFoundResult()
        {
            // Arrange
            var userId = 999;
            _mockUserService.Setup(s => s.GetUserAsync(userId)).ReturnsAsync((UserModel)null);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateUser_ValidUpdate_ReturnsOkResult()
        {
            // Arrange
            var userId = 1;
            var updateModel = new UserUpdateModel { Id = userId, Username = "updateduser", Email = "updated@example.com" };
            _mockUserService.Setup(s => s.UpdateUserAsync(updateModel)).ReturnsAsync(new UserModel { Id = userId, Username = "updateduser", Email = "updated@example.com" });

            // Act
            var result = await _controller.UpdateUser(userId, updateModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserModel>(okResult.Value);
            Assert.Equal(updateModel.Username, returnValue.Username);
        }

        [Fact]
        public async Task UpdateUser_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var updateModel = new UserUpdateModel { Id = 2, Username = "updateduser", Email = "updated@example.com" };

            // Act
            var result = await _controller.UpdateUser(1, updateModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Id mismatch", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteUser_ExistingUser_ReturnsNoContent()
        {
            // Arrange
            var userId = 1;
            _mockUserService.Setup(s => s.DeleteUserAsync(userId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUser_NonExistingUser_ReturnsNotFound()
        {
            // Arrange
            var userId = 999;
            _mockUserService.Setup(s => s.DeleteUserAsync(userId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetCurrentUser_AuthorizedUser_ReturnsOkResult()
        {
            // Arrange
            var userId = 1;
            var userModel = new UserModel { Id = userId, Username = "testuser", Email = "test@example.com" };
            _mockUserService.Setup(s => s.GetUserAsync(userId)).ReturnsAsync(userModel);

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetCurrentUser_UnauthorizedUser_ReturnsUnauthorizedResult()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
