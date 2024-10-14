using FinancialManagementSystem.Core.Models;

namespace FinancialManagementSystem.Core.Interfaces
{
    // FinancialManagementSystem.Core/Interfaces/IGoalService.cs
    public interface IGoalService
    {
        Task<IEnumerable<GoalModel>> GetGoalsAsync(int userId);
        Task<GoalModel> GetGoalAsync(int id);
        Task<GoalModel> AddGoalAsync(GoalModel model);
        Task<GoalModel> UpdateGoalAsync(GoalModel model);
        Task DeleteGoalAsync(int id);
        Task<IEnumerable<string>> GetRecommendationsAsync(int goalId);
    }
}
