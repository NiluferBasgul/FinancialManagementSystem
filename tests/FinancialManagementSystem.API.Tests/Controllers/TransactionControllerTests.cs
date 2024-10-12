using FinancialManagementSystem.API.Controllers;
using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;
using TransactionResult = FinancialManagementSystem.Core.Models.TransactionResult;

namespace FinancialManagementSystem.Tests.Controllers
{
    public class TransactionControllerTests
    {
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly TransactionController _controller;

        public TransactionControllerTests()
        {
            _mockTransactionService = new Mock<ITransactionService>();
            _controller = new TransactionController(_mockTransactionService.Object);

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
        public async Task GetTransactions_ReturnsOkResult()
        {
            // Arrange
            var userId = 1;
            var transactions = new List<Transaction> { new Transaction { Id = 1, UserId = userId } };
            _mockTransactionService.Setup(s => s.GetTransactionsAsync(userId, 0, 10))
                .ReturnsAsync(transactions);

            // Act
            var result = await _controller.GetTransactions();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Transaction>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task AddTransaction_ValidTransaction_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var userId = 1;
            var model = new TransactionModel { Amount = 100, Description = "Test", Date = DateTime.Now, Category = "Test", Type = "Expense" };
            _mockTransactionService.Setup(s => s.AddTransactionAsync(userId, model))
                .ReturnsAsync(new TransactionResult { Succeeded = true, TransactionId = 1 });

            // Act
            var result = await _controller.AddTransaction(model);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetTransactions", createdAtActionResult.ActionName);
            var returnValue = Assert.IsType<TransactionResult>(createdAtActionResult.Value);
            Assert.True(returnValue.Succeeded);
            Assert.Equal(1, returnValue.TransactionId);
        }

        [Fact]
        public async Task UpdateTransaction_ValidUpdate_ReturnsNoContent()
        {
            // Arrange
            var userId = 1;
            var transactionId = 1;
            var model = new TransactionModel { Amount = 100, Description = "Updated", Date = DateTime.Now, Category = "Test", Type = "Expense" };
            _mockTransactionService.Setup(s => s.UpdateTransactionAsync(userId, transactionId, model))
                .ReturnsAsync(new TransactionResult { Succeeded = true });

            // Act
            var result = await _controller.UpdateTransaction(transactionId, model);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTransaction_ExistingTransaction_ReturnsNoContent()
        {
            // Arrange
            var userId = 1;
            var transactionId = 1;
            _mockTransactionService.Setup(s => s.DeleteTransactionAsync(userId, transactionId))
                .ReturnsAsync(new TransactionResult { Succeeded = true });

            // Act
            var result = await _controller.DeleteTransaction(transactionId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTransaction_NonExistingTransaction_ReturnsBadRequest()
        {
            // Arrange
            var userId = 1;
            var transactionId = 999;
            _mockTransactionService.Setup(s => s.DeleteTransactionAsync(userId, transactionId))
                .ReturnsAsync(new TransactionResult { Succeeded = false, Errors = new List<string> { "Transaction not found" } });

            // Act
            var result = await _controller.DeleteTransaction(transactionId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errors = Assert.IsType<List<string>>(badRequestResult.Value);
            Assert.Single(errors);
            Assert.Equal("Transaction not found", errors[0]);
        }
    }
}
