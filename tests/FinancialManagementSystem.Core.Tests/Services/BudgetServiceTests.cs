using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Services;
using FinancialManagementSystem.Core.Models;
using Moq;
using Xunit;
using FinancialManagementSystem.Core.Entities;

namespace FinancialManagementSystem.Core.Tests.Services
{

    public class BudgetServiceTests
    {
        private readonly Mock<IBudgetRepository> _mockBudgetRepository;
        private readonly BudgetService _budgetService;

        public BudgetServiceTests()
        {
            _mockBudgetRepository = new Mock<IBudgetRepository>();
            _budgetService = new BudgetService(_mockBudgetRepository.Object);
        }

        [Fact]
        public async Task CreateBudgetAsync_ValidBudget_ReturnsBudgetModel()
        {
            // Arrange
            var model = new BudgetModel { Name = "Test Budget", Amount = 1000, StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), UserId = 1 };
            _mockBudgetRepository.Setup(r => r.AddAsync(It.IsAny<Budget>())).ReturnsAsync(new Budget { Id = 1, Name = model.Name, Amount = model.Amount, StartDate = model.StartDate, EndDate = model.EndDate, UserId = model.UserId });

            // Act
            var result = await _budgetService.CreateBudgetAsync(model);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(model.Name, result.Name);
            Assert.Equal(model.Amount, result.Amount);
        }

        [Fact]
        public async Task GetBudgetsAsync_ReturnsUserBudgets()
        {
            // Arrange
            var userId = 1;
            var budgets = new List<Budget> { new Budget { Id = 1, UserId = userId, Name = "Test Budget" } };
            _mockBudgetRepository.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(budgets);

            // Act
            var result = await _budgetService.GetBudgetsAsync(userId);

            // Assert
            Assert.Single(result);
            Assert.Equal(userId, result.First().UserId);
        }

        // Add more tests for UpdateBudgetAsync and DeleteBudgetAsync
    }
}
