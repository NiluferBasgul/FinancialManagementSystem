using FinancialManagementSystem.Core.Data;
using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagementSystem.Infrastructure.Repositories
{
    public class IncomeRepository : IIncomeRepository
    {
        private readonly ApplicationDbContext _context;

        public IncomeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Income>> GetByUserIdAsync(int userId)
        {
            return await _context.Incomes
                .Where(i => i.UserId == userId)
                .OrderBy(i => i.Date)
                .ToListAsync();
        }

        public async Task<Income> GetByIdAsync(int id)
        {
            return await _context.Incomes.FindAsync(id);
        }

        public async Task<Income> AddAsync(Income income)
        {
            await _context.Incomes.AddAsync(income);
            await _context.SaveChangesAsync();
            return income;
        }

        public async Task UpdateAsync(Income income)
        {
            _context.Incomes.Update(income);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Income income)
        {
            _context.Incomes.Remove(income);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalIncomeForPeriodAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _context.Incomes
                .Where(i => i.UserId == userId && i.Date >= startDate && i.Date <= endDate)
                .SumAsync(i => i.Amount);
        }

        public decimal GetTotalIncomeByUserId(int userId)
        {
            return _context.Incomes
                .Where(i => i.UserId == userId)
                .Sum(i => i.Amount);
        }

        public async Task DeleteAllIncomesAsync()
        {
            var incomes = await _context.Incomes.ToListAsync();
            _context.Incomes.RemoveRange(incomes);
            await _context.SaveChangesAsync();
        }
    }
}