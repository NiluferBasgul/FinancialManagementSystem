using FinancialManagementSystem.Core.Entities;

namespace FinancialManagementSystem.Core.Interfaces
{
    // FinancialManagementSystem.Core/Interfaces/IGoalRepository.cs
    public interface IGoalRepository
    {
        Task<IEnumerable<Goal>> GetByUserIdAsync(int userId);
        Task<Goal> GetByIdAsync(int id);
        Task<Goal> AddAsync(Goal goal);
        Task UpdateAsync(Goal goal);
        Task DeleteAsync(int id);
    }
}
