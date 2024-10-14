using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Models;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface IExpenseService
    {
        Task<IEnumerable<Expense>> GetExpensesByUserAsync(int userId, DateTime from, DateTime to);
        Task<Expense> AddExpenseAsync(ExpenseModel model, int userId);
        Task UpdateExpenseAsync(ExpenseModel model);
        Task DeleteExpenseAsync(int id);

        FinancialSummaryResult GetFinancialSummary(int userId);

    }

}
