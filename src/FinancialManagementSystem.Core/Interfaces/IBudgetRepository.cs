using FinancialManagementSystem.Core.Entities;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface IBudgetRepository
    {
        Task<Budget> AddAsync(Budget budget);
        Task<Budget> GetByIdAsync(int id);
        Task<Budget> UpdateAsync(Budget budget);
        Task DeleteAsync(int id);
        Task<IEnumerable<Budget>> GetByUserIdAsync(int userId);
        void SaveNeedsBudget(decimal needsAmount, int userId);
    }
}
