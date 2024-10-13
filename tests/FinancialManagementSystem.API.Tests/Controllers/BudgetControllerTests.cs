using FinancialManagementSystem.API.Controllers;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace FinancialManagementSystem.Tests.Controllers
{
    public class BudgetControllerTests
    {
        private readonly Mock<IBudgetService> _mockBudgetService;
        private readonly BudgetController _controller;
        private readonly Mock<ILogger<BudgetController>> _logger;

        public BudgetControllerTests()
        {
            _mockBudgetService = new Mock<IBudgetService>();
            _logger = new Mock<ILogger<BudgetController>>();
            _controller = new BudgetController(_mockBudgetService.Object, _logger.Object);

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
        public async Task GetBudgets_ReturnsOkResult()
        {
            // Arrange
            var userId = 1;
            var budgets = new List<BudgetModel> { new BudgetModel { Id = 1, UserId = userId, Name = "Test Budget" } };
            _mockBudgetService.Setup(s => s.GetBudgetsAsync(userId)).ReturnsAsync(budgets);

            // Act
            var result = await _controller.GetBudgets();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<BudgetModel>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task AddBudget_ValidBudget_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var userId = 1;
            var model = new BudgetModel { Name = "New Budget", Amount = 1000, StartDate = System.DateTime.Now, EndDate = System.DateTime.Now.AddMonths(1) };
            _mockBudgetService.Setup(s => s.AddBudgetAsync(userId, model))
                .ReturnsAsync(new BudgetResult { Succeeded = true, BudgetId = 1 });

            // Act
            var result = await _controller.AddBudget(model);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetBudgets", createdAtActionResult.ActionName);
            var returnValue = Assert.IsType<BudgetResult>(createdAtActionResult.Value);
            Assert.True(returnValue.Succeeded);
            Assert.Equal(1, returnValue.BudgetId);
        }

        [Fact]
        public async Task DeleteBudget_ReturnsNoContentResult()
        {
            // Arrange
            var budgetId = 1;
            _mockBudgetService.Setup(s => s.DeleteBudgetAsync(budgetId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteBudget(budgetId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        //[Fact]
        //public async Task GetNeedsBudget_ReturnsOkResult()
        //{
        //    // Arrange
        //    var budgetId = 1;
        //    var budget = new BudgetModel { Id = budgetId, Needs = 9685 };
        //    _mockBudgetService.Setup(s => s.GetBudgetAsync(budgetId)).ReturnsAsync(budget);

        //    // Act
        //    var result = await _controller.GetNeedsBudget(budgetId);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //}
    }
}
