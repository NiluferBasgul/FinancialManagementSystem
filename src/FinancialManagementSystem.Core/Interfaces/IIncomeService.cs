// FinancialManagementSystem.Core/Interfaces/IIncomeService.cs
using FinancialManagementSystem.Core.Models;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface IIncomeService
    {
        Task<IEnumerable<IncomeModel>> GetIncomesAsync(int userId);
        Task<IncomeModel> GetIncomeAsync(int id);
        Task<IncomeModel> AddIncomeAsync(IncomeModel model);
        Task<IncomeModel> UpdateIncomeAsync(IncomeModel model);
        Task DeleteIncomeAsync(int id);
        Task<decimal> GetTotalIncomeForPeriodAsync(int userId, DateTime startDate, DateTime endDate);
        Task DeleteAllIncomesAsync();
    }
}