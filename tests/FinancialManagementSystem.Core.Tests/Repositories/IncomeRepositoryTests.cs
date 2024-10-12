using FinancialManagementSystem.Core.Data;
using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FinancialManagementSystem.Tests.Repositories
{
    public class IncomeRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> GetInMemoryDbContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetByUserIdAsync_ReturnsIncomeForSpecificUser()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var userId = 1;

            using (var context = new ApplicationDbContext(options))
            {
                context.Incomes.AddRange(
                    new Income { Id = 1, UserId = userId, Amount = 1000, Date = DateTime.Now.AddDays(-10), Category = "Get", Description = "Test" },
                new Income { Id = 2, UserId = userId, Amount = 2000, Date = DateTime.Now.AddDays(-5), Category = "Get", Description = "Test" },
                new Income { Id = 3, UserId = 2, Amount = 1500, Date = DateTime.Now, Category = "Get", Description = "Test" }
                );
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new IncomeRepository(context);
                var result = await repository.GetByUserIdAsync(userId);

                // Assert
                Assert.Equal(2, result.Count());
                Assert.All(result, i => Assert.Equal(userId, i.UserId));
            }
        }

        [Fact]
        public async Task GetByIdAsync_ExistingIncome_ReturnsIncome()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var incomeId = 1;

            using (var context = new ApplicationDbContext(options))
            {
                context.Incomes.Add(new Income { Id = incomeId, UserId = 1, Amount = 1000, Date = DateTime.Now, Category = "Get", Description ="Test" });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new IncomeRepository(context);
                var result = await repository.GetByIdAsync(incomeId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1000, result.Amount);
            }
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingIncome_ReturnsNull()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var nonExistingIncomeId = 999;

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new IncomeRepository(context);
                var result = await repository.GetByIdAsync(nonExistingIncomeId);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task AddAsync_ValidIncome_AddsToDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var newIncome = new Income
            {
                UserId = 1,
                Amount = 5000,
                Date = DateTime.Now,
                Description = "Test Income",
                Category = "Test"
            };

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new IncomeRepository(context);
                var result = await repository.AddAsync(newIncome);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var addedIncome = await context.Incomes.FirstOrDefaultAsync(i => i.Description == "Test Income");
                Assert.NotNull(addedIncome);
                Assert.Equal(5000, addedIncome.Amount);
            }
        }

        [Fact]
        public async Task UpdateAsync_ExistingIncome_UpdatesIncomeInDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var incomeId = 1;

            using (var context = new ApplicationDbContext(options))
            {
                context.Incomes.Add(new Income { Id = incomeId, UserId = 1, Amount = 1000, Description = "Old Income", Date = DateTime.Now, Category = "Update" });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new IncomeRepository(context);
                var incomeToUpdate = await repository.GetByIdAsync(incomeId);
                incomeToUpdate.Amount = 2000;
                incomeToUpdate.Description = "Updated Income";
                await repository.UpdateAsync(incomeToUpdate);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var updatedIncome = await context.Incomes.FindAsync(incomeId);
                Assert.NotNull(updatedIncome);
                Assert.Equal(2000, updatedIncome.Amount);
                Assert.Equal("Updated Income", updatedIncome.Description);
            }
        }

        [Fact]
        public async Task DeleteAsync_ExistingIncome_RemovesFromDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var incomeId = 1;

            using (var context = new ApplicationDbContext(options))
            {
                context.Incomes.Add(new Income { Id = incomeId, UserId = 1, Amount = 1000, Description = "Income to Delete", Date = DateTime.Now, Category = "Delete" });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new IncomeRepository(context);
                var incomeToDelete = await repository.GetByIdAsync(incomeId);
                await repository.DeleteAsync(incomeToDelete);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var deletedIncome = await context.Incomes.FindAsync(incomeId);
                Assert.Null(deletedIncome);
            }
        }

        [Fact]
        public async Task GetTotalIncomeForPeriodAsync_ReturnsCorrectSum()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var userId = 1;
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now;

            using (var context = new ApplicationDbContext(options))
            {
                context.Incomes.AddRange(
                    new Income { Id = 1, UserId = userId, Amount = 1000, Date = DateTime.Now.AddDays(-25), Category = "Get", Description = "Test" },
                new Income { Id = 2, UserId = userId, Amount = 2000, Date = DateTime.Now.AddDays(-10), Category = "Get  " , Description = "Test" },
                new Income { Id = 3, UserId = userId, Amount = 3000, Date = DateTime.Now.AddDays(-5), Category = "Get", Description = "Test" }
                );
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new IncomeRepository(context);
                var result = await repository.GetTotalIncomeForPeriodAsync(userId, startDate, endDate);

                // Assert
                Assert.Equal(6000, result);
            }
        }

        [Fact]
        public void GetTotalIncomeByUserId_ReturnsCorrectTotal()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var userId = 1;

            using (var context = new ApplicationDbContext(options))
            {
                context.Incomes.AddRange(
                    new Income { Id = 1, UserId = userId, Amount = 1000, Date = DateTime.Now.AddDays(-20), Category = "Get", Description = "Test" },
                new Income { Id = 2, UserId = userId, Amount = 3000, Date = DateTime.Now.AddDays(-10), Category = "Get" , Description = "Test" },
                new Income { Id = 3, UserId = userId, Amount = 2000, Date = DateTime.Now.AddDays(-5), Category = "Get", Description = "Test" }
                );
                context.SaveChanges();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new IncomeRepository(context);
                var result = repository.GetTotalIncomeByUserId(userId);

                // Assert
                Assert.Equal(6000, result);
            }
        }
    }
}
