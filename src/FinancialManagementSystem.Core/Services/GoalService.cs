using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Interfaces.FinancialManagementSystem.Core.Services;
using FinancialManagementSystem.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialManagementSystem.Core.Services
{
    public class GoalService : IGoalService
    {
        private readonly IGoalRepository _goalRepository;
        private readonly ILogger<GoalService> _logger;
        private readonly ICacheService _cacheService;

        public GoalService(IGoalRepository goalRepository, ILogger<GoalService> logger, ICacheService cacheService)
        {
            _goalRepository = goalRepository;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<GoalModel>> GetGoalsAsync(int userId)
        {
            try
            {
                string cacheKey = $"goals_{userId}";
                return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
                {
                    var goals = await _goalRepository.GetByUserIdAsync(userId);
                    _logger.LogInformation($"Retrieved {goals.Count()} goals for user {userId} from database");
                    return goals.Select(MapToGoalModel);
                }, TimeSpan.FromMinutes(5));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving goals for user {userId}");
                throw;
            }
        }

        public async Task<GoalModel> GetGoalAsync(int id)
        {
            try
            {
                string cacheKey = $"goal_{id}";
                return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
                {
                    var goal = await _goalRepository.GetByIdAsync(id);
                    if (goal == null)
                    {
                        _logger.LogWarning($"Goal with id {id} not found");
                        return null;
                    }
                    _logger.LogInformation($"Retrieved goal {id} from database");
                    return MapToGoalModel(goal);
                }, TimeSpan.FromMinutes(5));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving goal with id {id}");
                throw;
            }
        }

        public async Task<GoalModel> AddGoalAsync(GoalModel model)
        {
            try
            {
                var goal = MapToGoal(model);
                var result = await _goalRepository.AddAsync(goal);
                await _cacheService.RemoveAsync($"goals_{model.UserId}");
                _logger.LogInformation($"Added new goal for user {model.UserId}: {model.Name}");
                return MapToGoalModel(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding goal for user {model.UserId}");
                throw;
            }
        }

        public async Task<GoalModel> UpdateGoalAsync(GoalModel model)
        {
            try
            {
                var goal = await _goalRepository.GetByIdAsync(model.Id);
                if (goal == null)
                {
                    _logger.LogWarning($"Goal with id {model.Id} not found for update");
                    return null;
                }

                goal.Name = model.Name;
                goal.TargetAmount = model.TargetAmount;
                goal.CurrentAmount = model.CurrentAmount;
                goal.StartDate = model.StartDate;
                goal.TargetDate = model.TargetDate;
                goal.IsCompleted = model.IsCompleted;

                await _goalRepository.UpdateAsync(goal);
                await _cacheService.RemoveAsync($"goals_{goal.UserId}");
                await _cacheService.RemoveAsync($"goal_{goal.Id}");
                _logger.LogInformation($"Updated goal: ID {goal.Id}, User {goal.UserId}");
                return MapToGoalModel(goal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating goal with id {model.Id}");
                throw;
            }
        }

        public async Task DeleteGoalAsync(int id)
        {
            try
            {
                var goal = await _goalRepository.GetByIdAsync(id);
                if (goal != null)
                {
                    await _goalRepository.DeleteAsync(id);
                    await _cacheService.RemoveAsync($"goals_{goal.UserId}");
                    await _cacheService.RemoveAsync($"goal_{id}");
                    _logger.LogInformation($"Deleted goal: ID {id}, User {goal.UserId}");
                }
                else
                {
                    _logger.LogWarning($"Attempted to delete non-existent goal with id {id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting goal with id {id}");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetRecommendationsAsync(int goalId)
        {
            try
            {
                var goal = await GetGoalAsync(goalId);
                if (goal == null)
                    return new List<string>();

                var recommendations = new List<string>();
                var remainingAmount = goal.TargetAmount - goal.CurrentAmount;
                var daysRemaining = (goal.TargetDate - DateTime.Now).Days;

                if (daysRemaining <= 0)
                {
                    recommendations.Add("Your goal deadline has passed. Consider setting a new target date.");
                }
                else
                {
                    var dailySavingsNeeded = remainingAmount / daysRemaining;
                    recommendations.Add($"To reach your goal, try to save {dailySavingsNeeded:C2} per day.");
                }

                // Add more personalized recommendations based on user's financial data
                // This is a simplified example

                _logger.LogInformation($"Generated recommendations for goal {goalId}");
                return recommendations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating recommendations for goal {goalId}");
                throw;
            }
        }

        private GoalModel MapToGoalModel(Goal goal)
        {
            return new GoalModel
            {
                Id = goal.Id,
                UserId = goal.UserId,
                Name = goal.Name,
                TargetAmount = goal.TargetAmount,
                CurrentAmount = goal.CurrentAmount,
                StartDate = goal.StartDate,
                TargetDate = goal.TargetDate,
                IsCompleted = goal.IsCompleted
            };
        }

        private Goal MapToGoal(GoalModel model)
        {
            return new Goal
            {
                Id = model.Id,
                UserId = model.UserId,
                Name = model.Name,
                TargetAmount = model.TargetAmount,
                CurrentAmount = model.CurrentAmount,
                StartDate = model.StartDate,
                TargetDate = model.TargetDate,
                IsCompleted = model.IsCompleted
            };
        }
    }
}