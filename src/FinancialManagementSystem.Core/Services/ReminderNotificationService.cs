// FinancialManagementSystem.API/Services/ReminderNotificationService.cs
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FinancialManagementSystem.API.Services
{
    public class ReminderNotificationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ReminderNotificationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var reminderService = scope.ServiceProvider.GetRequiredService<IReminderService>();
                    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                    var users = await userService.GetAllUsersAsync();

                    foreach (var user in users)
                    {
                        var upcomingReminders = await reminderService.GetUpcomingRemindersAsync(user.Id, DateTime.Now.AddDays(1));

                        foreach (var reminder in upcomingReminders)
                        {
                            // Send notification (e.g., email, push notification)
                            await SendNotification(user.Email, reminder);
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task SendNotification(string email, ReminderModel reminder)
        {
            // Implement notification logic (e.g., send email, push notification)
            // For this example, we'll just log the notification
            Console.WriteLine($"Sending notification to {email} for reminder: {reminder.Title}");
        }
    }
}