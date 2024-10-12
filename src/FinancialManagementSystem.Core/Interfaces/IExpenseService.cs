using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface IExpenseService
    {
        Task<IEnumerable<Expense>> GetExpensesByUserAsync(int userId, DateTime from, DateTime to);
        Task<Expense> AddExpenseAsync(ExpenseModel model, int userId);
        Task UpdateExpenseAsync(ExpenseModel model);
        Task DeleteExpenseAsync(int id);

        FinancialSummary GetFinancialSummary(int userId);

    }

}
