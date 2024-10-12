using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using FinancialManagementSystem.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FinancialManagementSystem.Core.Tests.Services
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<ILogger<TransactionService>> _mockLogger;
        private readonly TransactionService _transactionService;

        public TransactionServiceTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockLogger = new Mock<ILogger<TransactionService>>();
            _transactionService = new TransactionService(_mockTransactionRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetTransactionsAsync_ReturnsTransactions()
        {
            // Arrange
            var userId = 1;
            var transactions = new List<Transaction> { new Transaction { Id = 1, UserId = userId } };
            _mockTransactionRepository.Setup(r => r.GetTransactionsByUserIdAsync(userId, 0, 10))
                .ReturnsAsync(transactions);

            // Act
            var result = await _transactionService.GetTransactionsAsync(userId, 0, 10);

            // Assert
            Assert.Single(result);
            Assert.Equal(userId, result.First().UserId);
        }

        [Fact]
        public async Task GetTransactionsAsync_WhenRepositoryThrowsException_LogsError()
        {
            // Arrange
            var userId = 1;
            _mockTransactionRepository.Setup(r => r.GetTransactionsByUserIdAsync(userId, 0, 10))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _transactionService.GetTransactionsAsync(userId, 0, 10));
            Assert.Equal("Database error", ex.Message);
        }

        [Fact]
        public async Task AddTransactionAsync_ValidTransaction_ReturnsSuccessResult()
        {
            // Arrange
            var userId = 1;
            var model = new TransactionModel { Amount = 100, Description = "Test", Date = DateTime.Now, Category = "Test", Type = "Expense" };
            _mockTransactionRepository.Setup(r => r.AddAsync(It.IsAny<Transaction>())).ReturnsAsync(new Transaction { Id = 1 });

            // Act
            var result = await _transactionService.AddTransactionAsync(userId, model);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(1, result.TransactionId);
        }

        [Fact]
        public async Task AddTransactionAsync_WhenRepositoryThrowsException_ReturnsFailureResult()
        {
            // Arrange
            var userId = 1;
            var model = new TransactionModel { Amount = 100, Description = "Test", Date = DateTime.Now, Category = "Test", Type = "Expense" };
            _mockTransactionRepository.Setup(r => r.AddAsync(It.IsAny<Transaction>())).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _transactionService.AddTransactionAsync(userId, model);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Database error", result.Errors);
        }

        [Fact]
        public async Task UpdateTransactionAsync_ValidTransaction_ReturnsSuccessResult()
        {
            // Arrange
            var userId = 1;
            var transactionId = 1;
            var model = new TransactionModel { Amount = 150, Description = "Updated Transaction", Date = DateTime.Now, Category = "Updated", Type = "Income" };
            var existingTransaction = new Transaction { Id = transactionId, UserId = userId, Amount = 100, Description = "Old Transaction" };

            _mockTransactionRepository.Setup(r => r.GetByIdAsync(transactionId)).ReturnsAsync(existingTransaction);

            // Act
            var result = await _transactionService.UpdateTransactionAsync(userId, transactionId, model);

            // Assert
            Assert.True(result.Succeeded);
            _mockTransactionRepository.Verify(r => r.UpdateAsync(It.Is<Transaction>(t => t.Amount == 150 && t.Description == "Updated Transaction")), Times.Once);
        }

        [Fact]
        public async Task UpdateTransactionAsync_NonExistingTransaction_ReturnsFailureResult()
        {
            // Arrange
            var userId = 1;
            var transactionId = 999; // Assume transaction doesn't exist
            var model = new TransactionModel { Amount = 150, Description = "Updated Transaction", Date = DateTime.Now, Category = "Updated", Type = "Income" };

            _mockTransactionRepository.Setup(r => r.GetByIdAsync(transactionId)).ReturnsAsync((Transaction)null);

            // Act
            var result = await _transactionService.UpdateTransactionAsync(userId, transactionId, model);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Transaction not found or unauthorized", result.Errors);
        }

        [Fact]
        public async Task DeleteTransactionAsync_ExistingTransaction_SoftDeletesTransaction()
        {
            // Arrange
            var userId = 1;
            var transactionId = 1;
            var transaction = new Transaction { Id = transactionId, UserId = userId, IsDeleted = false };

            _mockTransactionRepository.Setup(r => r.GetByIdAsync(transactionId)).ReturnsAsync(transaction);

            // Act
            var result = await _transactionService.DeleteTransactionAsync(userId, transactionId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.True(transaction.IsDeleted);
            _mockTransactionRepository.Verify(r => r.UpdateAsync(transaction), Times.Once);
        }

        [Fact]
        public async Task DeleteTransactionAsync_NonExistingTransaction_ReturnsFailureResult()
        {
            // Arrange
            var userId = 1;
            var transactionId = 999; // Assume transaction does not exist
            _mockTransactionRepository.Setup(r => r.GetByIdAsync(transactionId)).ReturnsAsync((Transaction)null);

            // Act
            var result = await _transactionService.DeleteTransactionAsync(userId, transactionId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Transaction not found or unauthorized", result.Errors);
        }
    }
}
