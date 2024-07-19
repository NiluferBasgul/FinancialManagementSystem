using FinancialManagementSystem.API.Controllers;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace FinancialManagementSystem.Tests.Controllers
{
    public class ReminderControllerTests
    {
        private readonly Mock<IReminderService> _mockReminderService;
        private readonly ReminderController _controller;

        public ReminderControllerTests()
        {
            _mockReminderService = new Mock<IReminderService>();
            _controller = new ReminderController(_mockReminderService.Object);
            SetupControllerContext();
        }

        private void SetupControllerContext()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetReminders_ReturnsOkResultWithReminders()
        {
            // Arrange
            var userId = 1;
            var reminders = new List<ReminderModel>
            {
                new ReminderModel { Id = 1, UserId = userId, Title = "Pay rent", Description = "Monthly rent", DueDate = DateTime.Now.AddDays(5) },
                new ReminderModel { Id = 2, UserId = userId, Title = "Electricity bill", Description = "Quarterly bill", DueDate = DateTime.Now.AddDays(10) }
            };
            _mockReminderService.Setup(s => s.GetRemindersAsync(userId)).ReturnsAsync(reminders);

            // Act
            var result = await _controller.GetReminders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ReminderModel>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task AddReminder_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var reminderModel = new ReminderModel { UserId = 1, Title = "Pay rent", Description = "Monthly rent", DueDate = DateTime.Now.AddDays(5) };
            _mockReminderService.Setup(s => s.AddReminderAsync(It.IsAny<ReminderModel>())).ReturnsAsync(reminderModel);

            // Act
            var result = await _controller.AddReminder(reminderModel);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetReminder", createdAtActionResult.ActionName);
            var returnValue = Assert.IsType<ReminderModel>(createdAtActionResult.Value);
            Assert.Equal(reminderModel.Title, returnValue.Title);
        }

        // Add more tests for other actions
    }
}