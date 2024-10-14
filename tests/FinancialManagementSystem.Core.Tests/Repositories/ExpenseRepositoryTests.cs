using FinancialManagementSystem.Core.Data;
using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FinancialManagementSystem.Tests.Repositories
{
    public class ExpenseRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> GetInMemoryDbContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetExpensesByUserAsync_ReturnsExpensesWithinDateRange()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var userId = 1;
            var fromDate = DateTime.Now.AddDays(-5);
            var toDate = DateTime.Now;

            using (var context = new ApplicationDbContext(options))
            {
                context.Expenses.AddRange(
                    new Expense { Id = 1, UserId = userId, Amount = 100, Date = DateTime.Now.AddDays(-4), Category = "Test", Description = "Test", Pay = "Cash" },
                    new Expense { Id = 2, UserId = userId, Amount = 200, Date = DateTime.Now.AddDays(-2), Category = "Test1", Description = "Test1", Pay = "Cash" },
                    new Expense { Id = 3, UserId = 2, Amount = 300, Date = DateTime.Now.AddDays(-1), Category = "Test2", Description = "Test2", Pay = "Cash" }
                );
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new ExpenseRepository(context);
                var result = await repository.GetExpensesByUserAsync(userId, fromDate, toDate);

                // Assert
                Assert.Equal(2, result.Count());
                Assert.All(result, e => Assert.Equal(userId, e.UserId));
            }
        }

        [Fact]
        public async Task GetExpenseByIdAsync_ExistingExpense_ReturnsExpense()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var expenseId = 1;

            using (var context = new ApplicationDbContext(options))
            {
                context.Expenses.Add(new Expense { Id = expenseId, UserId = 1, Amount = 100, Date = DateTime.Now, Category = "Test", Description = "test", Pay = "Cash" });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new ExpenseRepository(context);
                var result = await repository.GetExpenseByIdAsync(expenseId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(100, result.Amount);
            }
        }

        [Fact]
        public async Task GetExpenseByIdAsync_NonExistingExpense_ReturnsNull()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var nonExistingExpenseId = 999;

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new ExpenseRepository(context);
                var result = await repository.GetExpenseByIdAsync(nonExistingExpenseId);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task AddExpenseAsync_ValidExpense_AddsToDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var newExpense = new Expense
            {
                UserId = 1,
                Amount = 500,
                Date = DateTime.Now,
                Description = "Test Expense",
                Category = "Test",
                Pay = "Cash"
            };

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new ExpenseRepository(context);
                var result = await repository.AddExpenseAsync(newExpense);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var addedExpense = await context.Expenses.FirstOrDefaultAsync(e => e.Description == "Test Expense");
                Assert.NotNull(addedExpense);
                Assert.Equal(500, addedExpense.Amount);
            }
        }

        [Fact]
        public async Task UpdateExpenseAsync_ExistingExpense_UpdatesExpenseInDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var expenseId = 1;

            using (var context = new ApplicationDbContext(options))
            {
                context.Expenses.Add(new Expense { Id = expenseId, UserId = 1, Amount = 100, Description = "Old Expense", Date = DateTime.Now, Category = "Test", Pay = "Cash" });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new ExpenseRepository(context);
                var expenseToUpdate = await repository.GetExpenseByIdAsync(expenseId);
                expenseToUpdate.Amount = 200;
                expenseToUpdate.Description = "Updated Expense";
                await repository.UpdateExpenseAsync(expenseToUpdate);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var updatedExpense = await context.Expenses.FindAsync(expenseId);
                Assert.NotNull(updatedExpense);
                Assert.Equal(200, updatedExpense.Amount);
                Assert.Equal("Updated Expense", updatedExpense.Description);
            }
        }

        [Fact]
        public async Task DeleteExpenseAsync_ExistingExpense_RemovesFromDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var expenseId = 1;

            using (var context = new ApplicationDbContext(options))
            {
                context.Expenses.Add(new Expense { Id = expenseId, UserId = 1, Amount = 100, Description = "Expense to Delete", Date = DateTime.Now, Category = "Test", Pay ="Cash" });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new ExpenseRepository(context);
                await repository.DeleteExpenseAsync(expenseId);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var deletedExpense = await context.Expenses.FindAsync(expenseId);
                Assert.Null(deletedExpense);
            }
        }

        [Fact]
        public void GetTotalExpensesByUserId_ExistingUser_ReturnsCorrectTotal()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var userId = 1;

            using (var context = new ApplicationDbContext(options))
            {
                context.Expenses.AddRange(
                    new Expense { Id = 1, UserId = userId, Amount = 100, Date = DateTime.Now.AddDays(-1), Category = "Test", Description = "Test", Pay = "Cash" },
                    new Expense { Id = 2, UserId = userId, Amount = 200, Date = DateTime.Now.AddDays(-2), Category = "Test1", Description = "Test1", Pay = "Cash" },
                    new Expense { Id = 3, UserId = 2, Amount = 300, Date = DateTime.Now, Category = "Test2", Description = "Test2", Pay = "Cash" }
                );
                context.SaveChanges();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new ExpenseRepository(context);
                var result = repository.GetTotalExpensesByUserId(userId);

                // Assert
                Assert.Equal(300, result);
            }
        }
    }
}
