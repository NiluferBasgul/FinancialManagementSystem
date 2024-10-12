using FinancialManagementSystem.Core.Data;
using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagementSystem.Core.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly ApplicationDbContext _context;

        public BudgetRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Budget> AddAsync(Budget budget)
        {
            if (budget.UserId <= 0 || string.IsNullOrEmpty(budget.Name) || budget.Amount <= 0)
            {
                throw new ArgumentException("Invalid budget data");
            }
            await _context.Budgets.AddAsync(budget);
            await _context.SaveChangesAsync();
            return budget;
        }

        public async Task<Budget> GetByIdAsync(int id)
        {
            return await _context.Budgets.FindAsync(id);
        }

        public async Task<Budget> UpdateAsync(Budget budget)
        {
            _context.Budgets.Update(budget);
            await _context.SaveChangesAsync();
            return budget;
        }

        public async Task DeleteAsync(int id)
        {
            var budget = await _context.Budgets.FindAsync(id);
            if (budget != null)
            {
                _context.Budgets.Remove(budget);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Budget>> GetByUserIdAsync(int userId)
        {
            return await _context.Budgets.Where(b => b.UserId == userId).ToListAsync();
        }

        public void SaveNeedsBudget(decimal needsAmount, int userId)
        {
            var budget = _context.Budgets.FirstOrDefault(b => b.UserId == userId);
            if (budget != null)
            {
                budget.Needs = needsAmount;
                _context.SaveChanges();
            }
        }
    }
}
