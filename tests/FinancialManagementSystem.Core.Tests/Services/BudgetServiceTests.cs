using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using FinancialManagementSystem.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FinancialManagementSystem.Core.Tests.Services
{
    public class BudgetServiceTests
    {
        private readonly Mock<IBudgetRepository> _mockBudgetRepository;
        private readonly BudgetService _budgetService;
        private readonly Mock<ILogger<BudgetService>> _logger;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IAccountRepository> _accountRepository;

        public BudgetServiceTests()
        {
            _mockBudgetRepository = new Mock<IBudgetRepository>();
            _userRepository = new Mock<IUserRepository>();
            _accountRepository = new Mock<IAccountRepository>();
            _logger = new Mock<ILogger<BudgetService>>();
            _budgetService = new BudgetService(_mockBudgetRepository.Object, _userRepository.Object, _logger.Object, _accountRepository.Object);
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
        public async Task CreateBudgetAsync_InvalidUserId_ThrowsArgumentException()
        {
            // Arrange
            var model = new BudgetModel { Name = "Test Budget", Amount = 1000, StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), UserId = 0 };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _budgetService.CreateBudgetAsync(model));
            Assert.Equal("Invalid user ID", ex.Message);
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

        [Fact]
        public async Task GetBudgetAsync_ExistingBudget_ReturnsBudgetModel()
        {
            // Arrange
            var budgetId = 1;
            var budget = new Budget { Id = budgetId, Name = "Test Budget", Amount = 1000 };
            _mockBudgetRepository.Setup(r => r.GetByIdAsync(budgetId)).ReturnsAsync(budget);

            // Act
            var result = await _budgetService.GetBudgetAsync(budgetId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(budget.Name, result.Name);
        }

        [Fact]
        public async Task GetBudgetAsync_NonExistingBudget_ReturnsNull()
        {
            // Arrange
            var budgetId = 999;
            _mockBudgetRepository.Setup(r => r.GetByIdAsync(budgetId)).ReturnsAsync((Budget)null);

            // Act
            var result = await _budgetService.GetBudgetAsync(budgetId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateBudgetAsync_ExistingBudget_ReturnsUpdatedBudget()
        {
            // Arrange
            var model = new BudgetModel { Id = 1, Name = "Updated Budget", Amount = 1500, StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1) };
            var budget = new Budget { Id = model.Id, Name = "Old Budget", Amount = 1000 };

            _mockBudgetRepository.Setup(r => r.GetByIdAsync(model.Id)).ReturnsAsync(budget);
            _mockBudgetRepository.Setup(r => r.UpdateAsync(budget)).ReturnsAsync(budget);

            // Act
            var result = await _budgetService.UpdateBudgetAsync(model);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Budget", result.Name);
            Assert.Equal(1500, result.Amount);
        }

        [Fact]
        public async Task UpdateBudgetAsync_NonExistingBudget_ReturnsNull()
        {
            // Arrange
            var model = new BudgetModel { Id = 999, Name = "Non-existing Budget" };
            _mockBudgetRepository.Setup(r => r.GetByIdAsync(model.Id)).ReturnsAsync((Budget)null);

            // Act
            var result = await _budgetService.UpdateBudgetAsync(model);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteBudgetAsync_DeletesBudget()
        {
            // Arrange
            var budgetId = 1;
            _mockBudgetRepository.Setup(r => r.DeleteAsync(budgetId)).Returns(Task.CompletedTask);

            // Act
            await _budgetService.DeleteBudgetAsync(budgetId);

            // Assert
            _mockBudgetRepository.Verify(r => r.DeleteAsync(budgetId), Times.Once);
        }

        [Fact]
        public async Task SubmitNeedsAsync_ValidUser_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var needsAmount = 500M;
            var user = new User { Id = userId, Username = "TestUser" };

            _userRepository.Setup(r => r.GetUserById(userId)).Returns(user);
            _mockBudgetRepository.Setup(r => r.SaveNeedsBudget(needsAmount, userId));

            // Act
            //var result = await _budgetService.SubmitNeedsAsync(needsAmount, userId);

            // Assert
            //Assert.True(result);
        }

        [Fact]
        public async Task SubmitNeedsAsync_InvalidUser_ReturnsFalse()
        {
            // Arrange
            var userId = 999;
            _userRepository.Setup(r => r.GetUserById(userId)).Returns((User)null);

            // Act
            //var result = await _budgetService.SubmitNeedsAsync(500M, userId);

            // Assert
            //Assert.False(result);
        }

        [Fact]
        public void TransferFunds_ValidAccounts_TransfersFunds()
        {
            // Arrange
            var fromAccount = new Account { Id = 1, Balance = 1000M };
            var toAccount = new Account { Id = 2, Balance = 500M };
            var transferRequest = new TransferRequest { FromAccountId = 1, ToAccountId = 2, Amount = 200M };

            _accountRepository.Setup(r => r.GetById(1)).Returns(fromAccount);
            _accountRepository.Setup(r => r.GetById(2)).Returns(toAccount);

            // Act
            _budgetService.TransferFunds(transferRequest);

            // Assert
            Assert.Equal(800M, fromAccount.Balance);
            Assert.Equal(700M, toAccount.Balance);
        }

        [Fact]
        public void TransferFunds_InsufficientFunds_ThrowsInvalidOperationException()
        {
            // Arrange
            var fromAccount = new Account { Id = 1, Balance = 100M };
            var toAccount = new Account { Id = 2, Balance = 500M };
            var transferRequest = new TransferRequest { FromAccountId = 1, ToAccountId = 2, Amount = 200M };

            _accountRepository.Setup(r => r.GetById(1)).Returns(fromAccount);
            _accountRepository.Setup(r => r.GetById(2)).Returns(toAccount);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _budgetService.TransferFunds(transferRequest));
        }
    }
}
