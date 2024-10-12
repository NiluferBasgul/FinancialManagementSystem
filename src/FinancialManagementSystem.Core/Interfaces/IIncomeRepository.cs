// FinancialManagementSystem.Core/Interfaces/IIncomeRepository.cs
using FinancialManagementSystem.Core.Entities;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface IIncomeRepository
    {
        Task<IEnumerable<Income>> GetByUserIdAsync(int userId);
        Task<Income> GetByIdAsync(int id);
        Task<Income> AddAsync(Income income);
        Task UpdateAsync(Income income);
        Task DeleteAsync(Income income);
        Task<decimal> GetTotalIncomeForPeriodAsync(int userId, DateTime startDate, DateTime endDate);
        decimal GetTotalIncomeByUserId(int userId);
    }
}