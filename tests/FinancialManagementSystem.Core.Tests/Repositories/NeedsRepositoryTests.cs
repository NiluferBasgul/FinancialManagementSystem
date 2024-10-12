using FinancialManagementSystem.Core.Data;
using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FinancialManagementSystem.Tests.Repositories
{
    public class NeedsRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> GetInMemoryDbContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void SaveNeedsBudget_ExistingBudget_UpdatesNeedsAmount()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var userId = 1;
            using (var context = new ApplicationDbContext(options))
            {
                context.Budgets.Add(new Budget { Id = 1, UserId = userId, Name = "Test Budget", Needs = 500 });
                context.SaveChanges();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new NeedsRepository(context);
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

        [Fact]
        public void SaveNeedsBudget_NonExistingBudget_DoesNothing()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var userId = 1;

            // No budget added in the setup

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new NeedsRepository(context);
                repository.SaveNeedsBudget(700, userId);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var nonExistingBudget = context.Budgets.FirstOrDefault(b => b.UserId == userId);
                Assert.Null(nonExistingBudget);
            }
        }
    }
}
