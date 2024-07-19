using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Repositories;
using FinancialManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FinancialManagementSystem.Tests.Repositories
{
    public class BudgetRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> GetInMemoryDbContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetByUserIdAsync_ReturnsBudgetsForUser()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var userId = 1;
            using (var context = new ApplicationDbContext(options))
            {
                context.Budgets.AddRange(
                    new Budget { Id = 1, UserId = userId, Name = "Budget 1", Amount = 1000 },
                    new Budget { Id = 2, UserId = userId, Name = "Budget 2", Amount = 2000 },
                    new Budget { Id = 3, UserId = 2, Name = "Other user budget", Amount = 3000 }
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
            }
        }

        [Fact]
        public async Task AddAsync_NewBudget_AddsBudgetToDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var newBudget = new Budget { UserId = 1, Name = "New Budget", Amount = 1500 };

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new BudgetRepository(context);
                await repository.AddAsync(newBudget);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var addedBudget = await context.Budgets.FirstOrDefaultAsync(b => b.Name == "New Budget");
                Assert.NotNull(addedBudget);
                Assert.Equal(1500, addedBudget.Amount);
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
                budgetToUpdate.Amount = 2000;
                await repository.UpdateAsync(budgetToUpdate);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var updatedBudget = await context.Budgets.FindAsync(budgetId);
                Assert.NotNull(updatedBudget);
                Assert.Equal("Updated Budget", updatedBudget.Name);
                Assert.Equal(2000, updatedBudget.Amount);
            }
        }

        [Fact]
        public async Task DeleteAsync_ExistingBudget_RemovesBudgetFromDatabase()
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
        }
    }
}