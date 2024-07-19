// FinancialManagementSystem.Core/Interfaces/IReminderRepository.cs
using FinancialManagementSystem.Core.Entities;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface IReminderRepository
    {
        Task<IEnumerable<Reminder>> GetByUserIdAsync(int userId);
        Task<Reminder> GetByIdAsync(int id);
        Task<Reminder> AddAsync(Reminder reminder);
        Task UpdateAsync(Reminder reminder);
        Task DeleteAsync(Reminder reminder);
        Task<IEnumerable<Reminder>> GetUpcomingRemindersAsync(int userId, DateTime endDate);
    }
}