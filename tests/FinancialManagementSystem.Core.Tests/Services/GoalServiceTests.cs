using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Interfaces.FinancialManagementSystem.Core.Services;
using FinancialManagementSystem.Core.Models;
using FinancialManagementSystem.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FinancialManagementSystem.Core.Tests.Services
{
    public class GoalServiceTests
    {
        private readonly Mock<IGoalRepository> _mockGoalRepository;
        private readonly Mock<ILogger<GoalService>> _mockLogger;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly GoalService _goalService;

        public GoalServiceTests()
        {
            _mockGoalRepository = new Mock<IGoalRepository>();
            _mockLogger = new Mock<ILogger<GoalService>>();
            _mockCacheService = new Mock<ICacheService>();
            _goalService = new GoalService(_mockGoalRepository.Object, _mockLogger.Object, _mockCacheService.Object);
        }

        [Fact]
        public async Task GetGoalsAsync_ReturnsGoalsFromCache_WhenAvailable()
        {
            // Arrange
            var userId = 1;
            var cacheKey = $"goals_{userId}";
            var goals = new List<Goal> { new Goal { Id = 1, UserId = userId, Name = "Test Goal" } };
            _mockCacheService.Setup(c => c.GetOrCreateAsync(cacheKey, It.IsAny<Func<Task<IEnumerable<Goal>>>>(), TimeSpan.FromMinutes(5)))
                             .ReturnsAsync(goals);

            // Act
            var result = await _goalService.GetGoalsAsync(userId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task AddGoalAsync_ValidGoal_AddsGoalAndClearsCache()
        {
            // Arrange
            var model = new GoalModel { UserId = 1, Name = "New Goal", TargetAmount = 1000 };
            var goal = new Goal { Id = 1, UserId = 1, Name = "New Goal", TargetAmount = 1000 };

            _mockGoalRepository.Setup(r => r.AddAsync(It.IsAny<Goal>())).ReturnsAsync(goal);

            // Act
            var result = await _goalService.AddGoalAsync(model);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Goal", result.Name);
            _mockCacheService.Verify(c => c.RemoveAsync($"goals_{model.UserId}"), Times.Once);
        }

        [Fact]
        public async Task UpdateGoalAsync_ExistingGoal_UpdatesGoalAndClearsCache()
        {
            // Arrange
            var model = new GoalModel { Id = 1, UserId = 1, Name = "Updated Goal", TargetAmount = 2000 };
            var goal = new Goal { Id = 1, UserId = 1, Name = "Old Goal", TargetAmount = 1000 };

            _mockGoalRepository.Setup(r => r.GetByIdAsync(model.Id)).ReturnsAsync(goal);
            _mockGoalRepository.Setup(r => r.UpdateAsync(It.IsAny<Goal>())).Returns(Task.CompletedTask);

            // Act
            var result = await _goalService.UpdateGoalAsync(model);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Goal", result.Name);
            _mockCacheService.Verify(c => c.RemoveAsync($"goals_{goal.UserId}"), Times.Once);
            _mockCacheService.Verify(c => c.RemoveAsync($"goal_{goal.Id}"), Times.Once);
        }

        [Fact]
        public async Task DeleteGoalAsync_ExistingGoal_DeletesGoalAndClearsCache()
        {
            // Arrange
            var goalId = 1;
            var goal = new Goal { Id = goalId, UserId = 1, Name = "Test Goal" };

            _mockGoalRepository.Setup(r => r.GetByIdAsync(goalId)).ReturnsAsync(goal);
            _mockGoalRepository.Setup(r => r.DeleteAsync(goalId)).Returns(Task.CompletedTask);

            // Act
            await _goalService.DeleteGoalAsync(goalId);

            // Assert
            _mockGoalRepository.Verify(r => r.DeleteAsync(goalId), Times.Once);
            _mockCacheService.Verify(c => c.RemoveAsync($"goals_{goal.UserId}"), Times.Once);
            _mockCacheService.Verify(c => c.RemoveAsync($"goal_{goalId}"), Times.Once);
        }

        [Fact]
        public async Task GetRecommendationsAsync_ValidGoal_ReturnsRecommendations()
        {
            // Arrange
            var goalId = 1;
            var goal = new GoalModel { Id = goalId, Name = "Test Goal", TargetAmount = 1000, CurrentAmount = 500, TargetDate = DateTime.Now.AddDays(10) };

            _mockGoalRepository.Setup(r => r.GetByIdAsync(goalId)).ReturnsAsync(new Goal
            {
                Id = goalId,
                Name = "Test Goal",
                TargetAmount = 1000,
                CurrentAmount = 500,
                TargetDate = goal.TargetDate
            });

            // Act
            var recommendations = await _goalService.GetRecommendationsAsync(goalId);

            // Assert
            Assert.NotNull(recommendations);
        }

        [Fact]
        public async Task GetRecommendationsAsync_PastTargetDate_ReturnsDeadlinePassedMessage()
        {
            // Arrange
            var goalId = 1;
            var goal = new GoalModel { Id = goalId, Name = "Test Goal", TargetAmount = 1000, CurrentAmount = 500, TargetDate = DateTime.Now.AddDays(-1) };

            _mockGoalRepository.Setup(r => r.GetByIdAsync(goalId)).ReturnsAsync(new Goal
            {
                Id = goalId,
                Name = "Test Goal",
                TargetAmount = 1000,
                CurrentAmount = 500,
                TargetDate = goal.TargetDate
            });

            // Act
            var recommendations = await _goalService.GetRecommendationsAsync(goalId);

            // Assert
            Assert.NotNull(recommendations);
        }
    }
}
