using FinancialManagementSystem.API.Controllers;
using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace FinancialManagementSystem.Tests.Controllers
{
    public class ExpenseControllerTests
    {
        private readonly Mock<IExpenseService> _mockExpenseService;
        private readonly Mock<IBudgetService> _mockBudgetService;
        private readonly ExpenseController _controller;

        public ExpenseControllerTests()
        {
            _mockExpenseService = new Mock<IExpenseService>();
            _mockBudgetService = new Mock<IBudgetService>();
            _controller = new ExpenseController(_mockExpenseService.Object, _mockBudgetService.Object);

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
        public async Task GetExpenses_ReturnsOkResult()
        {
            // Arrange
            var userId = 1;
            var from = DateTime.Now.AddDays(-30);
            var to = DateTime.Now;
            var expenses = new List<Expense>
            {
                new Expense { Id = 1, Amount = 100, Description = "Test Expense" }
            };

            _mockExpenseService.Setup(s => s.GetExpensesByUserAsync(userId, from, to)).ReturnsAsync(expenses);

            // Act
            var result = await _controller.GetExpenses(from, to);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Expense>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task AddExpense_ValidExpense_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var userId = 1;
            var model = new ExpenseModel { Id = 1, Amount = 100, Description = "New Expense", Date = DateTime.Now, Category = "Test", Pay = "Cash" };
            var expenseEntity = new Expense { Id = 1, Amount = 100, Description = "New Expense", Date = DateTime.Now, Category = "Test", Pay = "Cash" };

            _mockExpenseService.Setup(s => s.AddExpenseAsync(It.IsAny<ExpenseModel>(), userId)).ReturnsAsync(expenseEntity);

            // Act
            var result = await _controller.AddExpense(model);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task UpdateExpense_ValidExpense_ReturnsNoContentResult()
        {
            // Arrange
            var model = new ExpenseModel { Id = 1, Amount = 200, Description = "Updated Expense", Date = DateTime.Now, Category = "Updated", Pay = "Cash" };
            _mockExpenseService.Setup(s => s.UpdateExpenseAsync(model)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateExpense(1, model);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteExpense_ValidExpense_ReturnsNoContentResult()
        {
            // Arrange
            var expenseId = 1;
            _mockExpenseService.Setup(s => s.DeleteExpenseAsync(expenseId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteExpense(expenseId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void GetFinancialSummary_ReturnsOkResult()
        {
            // Arrange
            var userId = 1;
            var summary = new FinancialSummaryResult
            {
                TotalIncome = 5000,
                TotalExpenses = 2000,
                TotalSavings = 3000
            };
            _mockExpenseService.Setup(s => s.GetFinancialSummary(userId)).Returns(summary);

            // Act
            var result = _controller.GetFinancialSummary();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<FinancialSummaryResult>(okResult.Value);
            Assert.Equal(5000, returnValue.TotalIncome);
            Assert.Equal(2000, returnValue.TotalExpenses);
            Assert.Equal(3000, returnValue.TotalSavings);
        }
    }
}
