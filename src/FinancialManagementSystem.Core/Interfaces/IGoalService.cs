using FinancialManagementSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
