using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using FinancialManagementSystem.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FinancialManagementSystem.Tests.Services
{
    public class ReminderServiceTests
    {
        private readonly Mock<IReminderRepository> _mockReminderRepository;
        private readonly Mock<ILogger<ReminderService>> _mockLogger;
        private readonly ReminderService _reminderService;

        public ReminderServiceTests()
        {
            _mockReminderRepository = new Mock<IReminderRepository>();
            _mockLogger = new Mock<ILogger<ReminderService>>();
            _reminderService = new ReminderService(_mockReminderRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetRemindersAsync_ReturnsReminderModels()
        {
            // Arrange
            var userId = 1;
            var reminders = new List<Reminder>
            {
                new Reminder { Id = 1, UserId = userId, Title = "Pay rent", Description = "Monthly rent", DueDate = DateTime.Now.AddDays(5) },
                new Reminder { Id = 2, UserId = userId, Title = "Electricity bill", Description = "Quarterly bill", DueDate = DateTime.Now.AddDays(10) }
            };
            _mockReminderRepository.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(reminders);

            // Act
            var result = await _reminderService.GetRemindersAsync(userId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, r => Assert.Equal(userId, r.UserId));
        }

        [Fact]
        public async Task AddReminderAsync_ReturnsAddedReminderModel()
        {
            // Arrange
            var reminderModel = new ReminderModel { UserId = 1, Title = "Pay rent", Description = "Monthly rent", DueDate = DateTime.Now.AddDays(5) };
            var reminder = new Reminder { Id = 1, UserId = 1, Title = "Pay rent", Description = "Monthly rent", DueDate = DateTime.Now.AddDays(5) };
            _mockReminderRepository.Setup(r => r.AddAsync(It.IsAny<Reminder>())).ReturnsAsync(reminder);

            // Act
            var result = await _reminderService.AddReminderAsync(reminderModel);

            // Assert
            Assert.Equal(1, result.Id);
            Assert.Equal(reminderModel.Title, result.Title);
            Assert.Equal(reminderModel.Description, result.Description);
        }
    }
}