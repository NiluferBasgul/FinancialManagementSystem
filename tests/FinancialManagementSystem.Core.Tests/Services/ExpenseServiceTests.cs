using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using FinancialManagementSystem.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FinancialManagementSystem.Core.Tests.Services
{
    public class ExpenseServiceTests
    {
        private readonly Mock<IExpenseRepository> _mockExpenseRepository;
        private readonly Mock<IIncomeRepository> _mockIncomeRepository;
        private readonly Mock<ILogger<ExpenseService>> _mockLogger;
        private readonly ExpenseService _expenseService;

        public ExpenseServiceTests()
        {
            _mockExpenseRepository = new Mock<IExpenseRepository>();
            _mockIncomeRepository = new Mock<IIncomeRepository>();
            _mockLogger = new Mock<ILogger<ExpenseService>>();
            _expenseService = new ExpenseService(_mockExpenseRepository.Object, _mockIncomeRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetExpensesByUserAsync_ValidUser_ReturnsExpenses()
        {
            // Arrange
            var userId = 1;
            var from = DateTime.Now.AddMonths(-1);
            var to = DateTime.Now;
            var expenses = new List<Expense>
            {
                new Expense { Id = 1, UserId = userId, Amount = 100 },
                new Expense { Id = 2, UserId = userId, Amount = 200 }
            };
            _mockExpenseRepository.Setup(r => r.GetExpensesByUserAsync(userId, from, to)).ReturnsAsync(expenses);

            // Act
            var result = await _expenseService.GetExpensesByUserAsync(userId, from, to);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(userId, result.First().UserId);
        }

        [Fact]
        public async Task AddExpenseAsync_ValidExpense_ReturnsAddedExpense()
        {
            // Arrange
            var userId = 1;
            var model = new ExpenseModel { Amount = 100, Description = "Test Expense", Date = DateTime.Now, Category = "Food" };
            var expense = new Expense { Id = 1, UserId = userId, Amount = 100, Description = "Test Expense", Date = model.Date, Category = "Food" };

            _mockExpenseRepository.Setup(r => r.AddExpenseAsync(It.IsAny<Expense>())).ReturnsAsync(expense);

            // Act
            var result = await _expenseService.AddExpenseAsync(model, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(model.Amount, result.Amount);
        }

        [Fact]
        public async Task AddExpenseAsync_ThrowsException_LogsError()
        {
            // Arrange
            var userId = 1;
            var model = new ExpenseModel { Amount = 100, Description = "Test Expense", Date = DateTime.Now, Category = "Food" };

            _mockExpenseRepository.Setup(r => r.AddExpenseAsync(It.IsAny<Expense>())).Throws(new Exception("Database Error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _expenseService.AddExpenseAsync(model, userId));
            Assert.Equal("Database Error", ex.Message);
        }

        [Fact]
        public async Task UpdateExpenseAsync_ExistingExpense_UpdatesExpense()
        {
            // Arrange
            var expenseId = 1;
            var model = new ExpenseModel { Id = expenseId, Amount = 200, Description = "Updated Expense", Date = DateTime.Now, Category = "Transport" };
            var expense = new Expense { Id = expenseId, UserId = 1, Amount = 100, Description = "Old Expense", Date = model.Date, Category = "Food" };

            _mockExpenseRepository.Setup(r => r.GetExpenseByIdAsync(expenseId)).ReturnsAsync(expense);

            // Act
            await _expenseService.UpdateExpenseAsync(model);

            // Assert
            _mockExpenseRepository.Verify(r => r.UpdateExpenseAsync(It.Is<Expense>(e => e.Amount == 200 && e.Description == "Updated Expense" && e.Category == "Transport")), Times.Once);
        }

        [Fact]
        public async Task DeleteExpenseAsync_ValidExpense_DeletesExpense()
        {
            // Arrange
            var expenseId = 1;

            _mockExpenseRepository.Setup(r => r.DeleteExpenseAsync(expenseId)).Returns(Task.CompletedTask);

            // Act
            await _expenseService.DeleteExpenseAsync(expenseId);

            // Assert
            _mockExpenseRepository.Verify(r => r.DeleteExpenseAsync(expenseId), Times.Once);
        }

        [Fact]
        public void GetFinancialSummary_ValidUser_ReturnsCorrectSummary()
        {
            // Arrange
            var userId = 1;
            var totalIncome = 3000M;
            var totalExpenses = 2000M;

            _mockIncomeRepository.Setup(r => r.GetTotalIncomeByUserId(userId)).Returns(totalIncome);
            _mockExpenseRepository.Setup(r => r.GetTotalExpensesByUserId(userId)).Returns(totalExpenses);

            // Act
            var result = _expenseService.GetFinancialSummary(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(totalIncome, result.TotalIncome);
            Assert.Equal(totalExpenses, result.TotalExpenses);
            Assert.Equal(1000M, result.TotalSavings);
        }

        [Fact]
        public void GetFinancialSummary_ZeroIncome_ReturnsNegativeSavings()
        {
            // Arrange
            var userId = 1;
            var totalIncome = 0M;
            var totalExpenses = 2000M;

            _mockIncomeRepository.Setup(r => r.GetTotalIncomeByUserId(userId)).Returns(totalIncome);
            _mockExpenseRepository.Setup(r => r.GetTotalExpensesByUserId(userId)).Returns(totalExpenses);

            // Act
            var result = _expenseService.GetFinancialSummary(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(totalIncome, result.TotalIncome);
            Assert.Equal(totalExpenses, result.TotalExpenses);
            Assert.Equal(-2000M, result.TotalSavings);
        }
    }
}
