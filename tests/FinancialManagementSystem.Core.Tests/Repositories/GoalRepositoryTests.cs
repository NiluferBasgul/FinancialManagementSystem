using FinancialManagementSystem.Core.Data;
using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FinancialManagementSystem.Tests.Repositories
{
    public class GoalRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> GetInMemoryDbContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetByUserIdAsync_ReturnsGoalsForSpecificUser()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var userId = 1;

            using (var context = new ApplicationDbContext(options))
            {
                context.Goals.AddRange(
                    new Goal { Id = 1, UserId = userId, Name = "Goal 1", TargetAmount = 500 },
                    new Goal { Id = 2, UserId = userId, Name = "Goal 2", TargetAmount = 1500 },
                    new Goal { Id = 3, UserId = 2, Name = "Other User's Goal", TargetAmount = 2000 } 
                );
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new GoalRepository(context);
                var result = await repository.GetByUserIdAsync(userId);

                // Assert
                Assert.Equal(2, result.Count());
                Assert.All(result, g => Assert.Equal(userId, g.UserId));
            }
        }

        [Fact]
        public async Task GetByIdAsync_ExistingGoal_ReturnsGoal()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var goalId = 1;

            using (var context = new ApplicationDbContext(options))
            {
                context.Goals.Add(new Goal { Id = goalId, UserId = 1, Name = "Test Goal", TargetAmount = 1000 });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new GoalRepository(context);
                var result = await repository.GetByIdAsync(goalId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("Test Goal", result.Name);
                Assert.Equal(1000, result.TargetAmount);
            }
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingGoal_ReturnsNull()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var nonExistingGoalId = 999;

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new GoalRepository(context);
                var result = await repository.GetByIdAsync(nonExistingGoalId);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task AddAsync_ValidGoal_AddsToDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var newGoal = new Goal
            {
                UserId = 1,
                Name = "New Goal",
                TargetAmount = 5000
            };

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new GoalRepository(context);
                var result = await repository.AddAsync(newGoal);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var addedGoal = await context.Goals.FirstOrDefaultAsync(g => g.Name == "New Goal");
                Assert.NotNull(addedGoal);
                Assert.Equal(5000, addedGoal.TargetAmount);
            }
        }

        [Fact]
        public async Task UpdateAsync_ExistingGoal_UpdatesGoalInDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var goalId = 1;

            using (var context = new ApplicationDbContext(options))
            {
                context.Goals.Add(new Goal { Id = goalId, UserId = 1, Name = "Old Goal", TargetAmount = 3000 });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new GoalRepository(context);
                var goalToUpdate = await repository.GetByIdAsync(goalId);
                goalToUpdate.Name = "Updated Goal";
                goalToUpdate.TargetAmount = 6000;
                await repository.UpdateAsync(goalToUpdate);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var updatedGoal = await context.Goals.FindAsync(goalId);
                Assert.NotNull(updatedGoal);
                Assert.Equal("Updated Goal", updatedGoal.Name);
                Assert.Equal(6000, updatedGoal.TargetAmount);
            }
        }

        [Fact]
        public async Task DeleteAsync_ExistingGoal_RemovesFromDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var goalId = 1;

            using (var context = new ApplicationDbContext(options))
            {
                context.Goals.Add(new Goal { Id = goalId, UserId = 1, Name = "Goal to Delete", TargetAmount = 1000 });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new GoalRepository(context);
                await repository.DeleteAsync(goalId);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var deletedGoal = await context.Goals.FindAsync(goalId);
                Assert.Null(deletedGoal);
            }
        }
    }
}
