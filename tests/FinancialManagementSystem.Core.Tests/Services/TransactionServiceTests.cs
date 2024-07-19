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
            _mockTransactionRepository.Setup(r => r.GetTransactionsByUserIdAsync(userId)).ReturnsAsync(transactions);

            // Act
            var result = await _transactionService.GetTransactionsAsync(userId);

            // Assert
            Assert.Single(result);
            Assert.Equal(userId, result.First().UserId);
        }

        [Fact]
        public async Task AddTransactionAsync_ValidTransaction_ReturnsSuccessResult()
        {
            // Arrange
            var userId = 1;
            var model = new TransactionModel { Amount = 100, Description = "Test", Date = DateTime.Now, Category = "Test", Type = TransactionType.Expense };
            _mockTransactionRepository.Setup(r => r.AddAsync(It.IsAny<Transaction>())).ReturnsAsync(new Transaction { Id = 1 });

            // Act
            var result = await _transactionService.AddTransactionAsync(userId, model);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(1, result.TransactionId);
        }
    }
}