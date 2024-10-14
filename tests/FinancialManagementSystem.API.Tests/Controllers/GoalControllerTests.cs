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
    public class GoalControllerTests
    {
        private readonly Mock<IGoalService> _mockGoalService;
        private readonly GoalController _controller;

        public GoalControllerTests()
        {
            _mockGoalService = new Mock<IGoalService>();
            _controller = new GoalController(_mockGoalService.Object);

            // Setup ClaimsPrincipal for authorized user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task GetGoals_ReturnsOkResult()
        {
            // Arrange
            var userId = 1;
            var goals = new List<GoalModel> { new GoalModel { Id = 1, Name = "Goal 1", UserId = userId } };
            _mockGoalService.Setup(s => s.GetGoalsAsync(userId)).ReturnsAsync(goals);

            // Act
            var result = await _controller.GetGoals();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<GoalModel>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetGoal_ExistingGoal_ReturnsOkResult()
        {
            // Arrange
            var goalId = 1;
            var goal = new GoalModel { Id = goalId, Name = "Goal 1" };
            _mockGoalService.Setup(s => s.GetGoalAsync(goalId)).ReturnsAsync(goal);

            // Act
            var result = await _controller.GetGoal(goalId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<GoalModel>(okResult.Value);
            Assert.Equal(goalId, returnValue.Id);
        }

        [Fact]
        public async Task GetGoal_NonExistingGoal_ReturnsNotFoundResult()
        {
            // Arrange
            var goalId = 999;
            _mockGoalService.Setup(s => s.GetGoalAsync(goalId)).ReturnsAsync((GoalModel)null);

            // Act
            var result = await _controller.GetGoal(goalId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddGoal_ValidGoal_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var model = new GoalModel { Name = "New Goal", UserId = 1 };
            _mockGoalService.Setup(s => s.AddGoalAsync(model)).ReturnsAsync(model);

            // Act
            var result = await _controller.AddGoal(model);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetGoal", createdAtActionResult.ActionName);
            var returnValue = Assert.IsType<GoalModel>(createdAtActionResult.Value);
            Assert.Equal(model.Name, returnValue.Name);
        }

        [Fact]
        public async Task UpdateGoal_ValidGoal_ReturnsNoContentResult()
        {
            // Arrange
            var model = new GoalModel { Id = 1, Name = "Updated Goal", UserId = 1 };
            _mockGoalService.Setup(s => s.UpdateGoalAsync(model)).ReturnsAsync(model);

            // Act
            var result = await _controller.UpdateGoal(1, model);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateGoal_NonExistingGoal_ReturnsNotFoundResult()
        {
            // Arrange
            var model = new GoalModel { Id = 1, Name = "Updated Goal", UserId = 1 };
            _mockGoalService.Setup(s => s.UpdateGoalAsync(model)).ReturnsAsync((GoalModel)null);

            // Act
            var result = await _controller.UpdateGoal(1, model);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteGoal_ReturnsNoContentResult()
        {
            // Arrange
            var goalId = 1;
            _mockGoalService.Setup(s => s.DeleteGoalAsync(goalId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteGoal(goalId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetRecommendations_ReturnsOkResult()
        {
            // Arrange
            var goalId = 1;
            var recommendations = new List<string> { "Recommendation 1", "Recommendation 2" };
            _mockGoalService.Setup(s => s.GetRecommendationsAsync(goalId)).ReturnsAsync(recommendations);

            // Act
            var result = await _controller.GetRecommendations(goalId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<string>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
    }
}
