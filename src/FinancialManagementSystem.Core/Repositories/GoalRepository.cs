using FinancialManagementSystem.Core.Data;
using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagementSystem.Core.Repositories
{
    public class GoalRepository : IGoalRepository
    {
        private readonly ApplicationDbContext _context;

        public GoalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Goal>> GetByUserIdAsync(int userId)
        {
            return await _context.Goals.Where(g => g.UserId == userId).ToListAsync();
        }

        public async Task<Goal> GetByIdAsync(int id)
        {
            return await _context.Goals.FindAsync(id);
        }

        public async Task<Goal> AddAsync(Goal goal)
        {
            await _context.Goals.AddAsync(goal);
            await _context.SaveChangesAsync();
            return goal;
        }

        public async Task UpdateAsync(Goal goal)
        {
            _context.Goals.Update(goal);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var goal = await _context.Goals.FindAsync(id);
            if (goal != null)
            {
                _context.Goals.Remove(goal);
                await _context.SaveChangesAsync();
            }
        }
    }
}
