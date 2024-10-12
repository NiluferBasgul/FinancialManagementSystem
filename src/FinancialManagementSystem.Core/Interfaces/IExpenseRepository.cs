using FinancialManagementSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface IExpenseRepository
    {
        Task<IEnumerable<Expense>> GetExpensesByUserAsync(int userId, DateTime from, DateTime to);
        Task<Expense> GetExpenseByIdAsync(int id);
        Task<Expense> AddExpenseAsync(Expense expense);
        Task UpdateExpenseAsync(Expense expense);
        Task DeleteExpenseAsync(int id);
        decimal GetTotalExpensesByUserId(int userId);
    }

}
