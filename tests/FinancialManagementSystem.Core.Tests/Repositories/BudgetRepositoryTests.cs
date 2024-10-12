using FinancialManagementSystem.Core.Data;
using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialManagementSystem.Tests.Repositories
{
    public class BudgetRepositoryTests
    {
        private static DbContextOptions<ApplicationDbContext> GetInMemoryDbContextOptions() =>
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

        [Fact]
        public async Task AddAsync_ValidBudget_AddsToDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var newBudget = new Budget
            {
                UserId = 1,
                Name = "Test Budget",
                Amount = 1000
            };

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new BudgetRepository(context);
                var result = await repository.AddAsync(newBudget);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var addedBudget = await context.Budgets.FirstOrDefaultAsync(b => b.Name == "Test Budget");
                Assert.NotNull(addedBudget);
                Assert.Equal(1000, addedBudget.Amount);
            }
        }

        [Fact]
        public async Task AddAsync_InvalidBudget_ThrowsArgumentException()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var invalidBudget = new Budget
            {
                UserId = 0, // Invalid UserId
                Name = "",  // Invalid Name
                Amount = -500 // Invalid Amount
            };

            // Act & Assert
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new BudgetRepository(context);
                await Assert.ThrowsAsync<ArgumentException>(async () => await repository.AddAsync(invalidBudget));
            }
        }

        [Fact]
        public async Task GetByIdAsync_ExistingBudget_ReturnsBudget()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var budgetId = 1;
            using (var context = new ApplicationDbContext(options))
            {
                context.Budgets.Add(new Budget { Id = budgetId, UserId = 1, Name = "Test Budget", Amount = 1000 });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new BudgetRepository(context);
                var result = await repository.GetByIdAsync(budgetId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Test Budget", result.Name);
                Assert.Equal(1000, result.Amount);
            }
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingBudget_ReturnsNull()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var nonExistingBudgetId = 999;

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new BudgetRepository(context);
                var result = await repository.GetByIdAsync(nonExistingBudgetId);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task UpdateAsync_ExistingBudget_UpdatesBudgetInDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var budgetId = 1;
            using (var context = new ApplicationDbContext(options))
            {
                context.Budgets.Add(new Budget { Id = budgetId, UserId = 1, Name = "Original Budget", Amount = 1000 });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new BudgetRepository(context);
                var budgetToUpdate = await repository.GetByIdAsync(budgetId);
                budgetToUpdate.Name = "Updated Budget";
                budgetToUpdate.Amount = 1500;
                await repository.UpdateAsync(budgetToUpdate);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var updatedBudget = await context.Budgets.FindAsync(budgetId);
                Assert.NotNull(updatedBudget);
                Assert.Equal("Updated Budget", updatedBudget.Name);
                Assert.Equal(1500, updatedBudget.Amount);
            }
        }

        [Fact]
        public async Task DeleteAsync_ExistingBudget_RemovesFromDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var budgetId = 1;
            using (var context = new ApplicationDbContext(options))
            {
                context.Budgets.Add(new Budget { Id = budgetId, UserId = 1, Name = "Budget to Delete", Amount = 1000 });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new BudgetRepository(context);
                await repository.DeleteAsync(budgetId);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var deletedBudget = await context.Budgets.FindAsync(budgetId);
                Assert.Null(deletedBudget);
            }
        }

        [Fact]
        public async Task GetByUserIdAsync_ExistingUser_ReturnsBudgets()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var userId = 1;
            using (var context = new ApplicationDbContext(options))
            {
                context.Budgets.AddRange(
                    new Budget { Id = 1, UserId = userId, Name = "Budget 1", Amount = 500 },
                    new Budget { Id = 2, UserId = userId, Name = "Budget 2", Amount = 1500 },
                    new Budget { Id = 3, UserId = 2, Name = "Budget for Other User", Amount = 2000 }
                );
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new BudgetRepository(context);
                var result = await repository.GetByUserIdAsync(userId);

                // Assert
                Assert.Equal(2, result.Count());
                Assert.All(result, b => Assert.Equal(userId, b.UserId));
            }
        }

        [Fact]
        public void SaveNeedsBudget_ExistingBudget_UpdatesNeedsAmount()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var userId = 1;
            using (var context = new ApplicationDbContext(options))
            {
                context.Budgets.Add(new Budget { Id = 1, UserId = userId, Name = "Test Budget", Needs = 0 });
                context.SaveChanges();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new BudgetRepository(context);
                repository.SaveNeedsBudget(700, userId);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var updatedBudget = context.Budgets.FirstOrDefault(b => b.UserId == userId);
                Assert.NotNull(updatedBudget);
                Assert.Equal(700, updatedBudget.Needs);
            }
        }
    }
}
