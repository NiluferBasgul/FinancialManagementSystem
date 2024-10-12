using FinancialManagementSystem.Core.Data;
using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagementSystem.Core.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ApplicationDbContext _context;

        public ExpenseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Expense>> GetExpensesByUserAsync(int userId, DateTime from, DateTime to)
        {
            return await _context.Expenses
                .Where(e => e.UserId == userId && e.Date >= from && e.Date <= to)
                .ToListAsync();
        }

        public async Task<Expense> GetExpenseByIdAsync(int id)
        {
            return await _context.Expenses.FindAsync(id);
        }

        public async Task<Expense> AddExpenseAsync(Expense expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            return expense;
        }

        public async Task UpdateExpenseAsync(Expense expense)
        {
            _context.Expenses.Update(expense);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteExpenseAsync(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
            }
        }

        public decimal GetTotalExpensesByUserId(int userId)
        {
            return _context.Expenses.Where(e => e.UserId == userId).Sum(e => e.Amount);
        }
    }
}
