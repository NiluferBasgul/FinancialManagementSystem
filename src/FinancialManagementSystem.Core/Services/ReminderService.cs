using Microsoft.Extensions.Logging;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using FinancialManagementSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace FinancialManagementSystem.Core.Services
{
    public class ReminderService : IReminderService
    {
        private readonly IReminderRepository _reminderRepository;
        private readonly ILogger<ReminderService> _logger;

        public ReminderService(IReminderRepository reminderRepository, ILogger<ReminderService> logger)
        {
            _reminderRepository = reminderRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ReminderModel>> GetRemindersAsync(int userId)
        {
            try
            {
                var reminders = await _reminderRepository.GetByUserIdAsync(userId);
                return reminders.Select(MapToReminderModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting reminders for user {userId}");
                throw;
            }
        }

        public async Task<ReminderModel> GetReminderAsync(int id)
        {
            try
            {
                var reminder = await _reminderRepository.GetByIdAsync(id);
                return reminder != null ? MapToReminderModel(reminder) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting reminder with id {id}");
                throw;
            }
        }

        public async Task<ReminderModel> AddReminderAsync(ReminderModel model)
        {
            try
            {
                var reminder = MapToReminder(model);
                var result = await _reminderRepository.AddAsync(reminder);
                _logger.LogInformation($"Reminder added for user {model.UserId}: {model.Title}");
                return MapToReminderModel(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding reminder for user {model.UserId}");
                throw;
            }
        }

        public async Task<ReminderModel> UpdateReminderAsync(ReminderModel model)
        {
            try
            {
                var reminder = await _reminderRepository.GetByIdAsync(model.Id);
                if (reminder == null)
                {
                    _logger.LogWarning($"Reminder with id {model.Id} not found for update");
                    return null;
                }

                reminder.Title = model.Title;
                reminder.Description = model.Description;
                reminder.DueDate = model.DueDate;
                reminder.IsCompleted = model.IsCompleted;

                await _reminderRepository.UpdateAsync(reminder);
                _logger.LogInformation($"Reminder updated: ID {model.Id}, User {model.UserId}");
                return MapToReminderModel(reminder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating reminder with id {model.Id}");
                throw;
            }
        }

        public async Task DeleteReminderAsync(int id)
        {
            try
            {
                var reminder = await _reminderRepository.GetByIdAsync(id);
                if (reminder != null)
                {
                    await _reminderRepository.DeleteAsync(reminder);
                    _logger.LogInformation($"Reminder deleted: ID {id}, User {reminder.UserId}");
                }
                else
                {
                    _logger.LogWarning($"Attempted to delete non-existent reminder with id {id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting reminder with id {id}");
                throw;
            }
        }

        public async Task<IEnumerable<ReminderModel>> GetUpcomingRemindersAsync(int userId, DateTime endDate)
        {
            try
            {
                var reminders = await _reminderRepository.GetUpcomingRemindersAsync(userId, endDate);
                _logger.LogInformation($"Retrieved {reminders.Count()} upcoming reminders for user {userId} until {endDate}");
                return reminders.Select(MapToReminderModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting upcoming reminders for user {userId} until {endDate}");
                throw;
            }
        }

        private ReminderModel MapToReminderModel(Reminder reminder)
        {
            return new ReminderModel
            {
                Id = reminder.Id,
                UserId = reminder.UserId,
                Title = reminder.Title,
                Description = reminder.Description,
                DueDate = reminder.DueDate,
                IsCompleted = reminder.IsCompleted
            };
        }

        private Reminder MapToReminder(ReminderModel model)
        {
            return new Reminder
            {
                Id = model.Id,
                UserId = model.UserId,
                Title = model.Title,
                Description = model.Description,
                DueDate = model.DueDate,
                IsCompleted = model.IsCompleted
            };
        }
    }
}