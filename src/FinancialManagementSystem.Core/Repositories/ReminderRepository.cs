// FinancialManagementSystem.Infrastructure/Repositories/ReminderRepository.cs
using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagementSystem.Infrastructure.Repositories
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly ApplicationDbContext _context;

        public ReminderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reminder>> GetByUserIdAsync(int userId)
        {
            return await _context.Reminders.Where(r => r.UserId == userId).ToListAsync();
        }

        public async Task<Reminder> GetByIdAsync(int id)
        {
            return await _context.Reminders.FindAsync(id);
        }

        public async Task<Reminder> AddAsync(Reminder reminder)
        {
            await _context.Reminders.AddAsync(reminder);
            await _context.SaveChangesAsync();
            return reminder;
        }

        public async Task UpdateAsync(Reminder reminder)
        {
            _context.Reminders.Update(reminder);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Reminder reminder)
        {
            _context.Reminders.Remove(reminder);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Reminder>> GetUpcomingRemindersAsync(int userId, DateTime endDate)
        {
            return await _context.Reminders
                .Where(r => r.UserId == userId && r.DueDate <= endDate && !r.IsCompleted)
                .OrderBy(r => r.DueDate)
                .ToListAsync();
        }
    }
}