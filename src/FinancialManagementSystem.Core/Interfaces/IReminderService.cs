// FinancialManagementSystem.Core/Interfaces/IReminderService.cs
using FinancialManagementSystem.Core.Models;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface IReminderService
    {
        Task<IEnumerable<ReminderModel>> GetRemindersAsync(int userId);
        Task<ReminderModel> GetReminderAsync(int id);
        Task<ReminderModel> AddReminderAsync(ReminderModel model);
        Task<ReminderModel> UpdateReminderAsync(ReminderModel model);
        Task DeleteReminderAsync(int id);
        Task<IEnumerable<ReminderModel>> GetUpcomingRemindersAsync(int userId, DateTime endDate);
    }
}