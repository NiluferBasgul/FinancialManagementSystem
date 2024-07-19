// FinancialManagementSystem.Infrastructure/Repositories/IncomeRepository.cs
using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Infrastructure.Data;
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
            return await _context.Incomes.Where(i => i.UserId == userId).ToListAsync();
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
    }
}