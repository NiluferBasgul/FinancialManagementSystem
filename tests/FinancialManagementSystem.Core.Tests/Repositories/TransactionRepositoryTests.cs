﻿using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Infrastructure.Data;
using FinancialManagementSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FinancialManagementSystem.Tests.Repositories
{
    public class TransactionRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> GetInMemoryDbContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetTransactionsByUserIdAsync_ReturnsUserTransactions()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var userId = 1;
            using (var context = new ApplicationDbContext(options))
            {
                context.Transactions.AddRange(
                    new Transaction { Id = 1, UserId = userId, Amount = 100, Description = "Test 1" },
                    new Transaction { Id = 2, UserId = userId, Amount = 200, Description = "Test 2" },
                    new Transaction { Id = 3, UserId = 2, Amount = 300, Description = "Other user" }
                );
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new TransactionRepository(context);
                var result = await repository.GetTransactionsByUserIdAsync(userId);

                // Assert
                Assert.Equal(2, result.Count());
                Assert.All(result, t => Assert.Equal(userId, t.UserId));
            }
        }

        [Fact]
        public async Task GetByIdAsync_ExistingTransaction_ReturnsTransaction()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var transactionId = 1;
            using (var context = new ApplicationDbContext(options))
            {
                context.Transactions.Add(new Transaction { Id = transactionId, UserId = 1, Amount = 100, Description = "Test" });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new TransactionRepository(context);
                var result = await repository.GetByIdAsync(transactionId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(100, result.Amount);
            }
        }

        [Fact]
        public async Task AddAsync_NewTransaction_AddsTransactionToDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var newTransaction = new Transaction
            {
                UserId = 1,
                Amount = 150,
                Description = "New Transaction",
                Date = DateTime.Now,
                Category = "Test Category", // Add this line
                Type = TransactionType.Expense // Add this line if it's a required field
            };

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new TransactionRepository(context);
                await repository.AddAsync(newTransaction);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var addedTransaction = await context.Transactions.FirstOrDefaultAsync(t => t.Description == "New Transaction");
                Assert.NotNull(addedTransaction);
                Assert.Equal(150, addedTransaction.Amount);
                Assert.Equal("Test Category", addedTransaction.Category);
            }
        }
        [Fact]
        public async Task UpdateAsync_ExistingTransaction_UpdatesTransactionInDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var transactionId = 1;
            using (var context = new ApplicationDbContext(options))
            {
                context.Transactions.Add(new Transaction { Id = transactionId, UserId = 1, Amount = 100, Description = "Original" });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new TransactionRepository(context);
                var transactionToUpdate = await repository.GetByIdAsync(transactionId);
                transactionToUpdate.Amount = 200;
                transactionToUpdate.Description = "Updated";
                await repository.UpdateAsync(transactionToUpdate);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var updatedTransaction = await context.Transactions.FindAsync(transactionId);
                Assert.NotNull(updatedTransaction);
                Assert.Equal(200, updatedTransaction.Amount);
                Assert.Equal("Updated", updatedTransaction.Description);
            }
        }

        [Fact]
        public async Task DeleteAsync_ExistingTransaction_RemovesTransactionFromDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var transactionId = 1;
            using (var context = new ApplicationDbContext(options))
            {
                context.Transactions.Add(new Transaction { Id = transactionId, UserId = 1, Amount = 100, Description = "To Delete" });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new TransactionRepository(context);
                var transactionToDelete = await repository.GetByIdAsync(transactionId);
                await repository.DeleteAsync(transactionToDelete);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var deletedTransaction = await context.Transactions.FindAsync(transactionId);
                Assert.Null(deletedTransaction);
            }
        }
    }
}