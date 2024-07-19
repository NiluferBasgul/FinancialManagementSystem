// FinancialManagementSystem.API/Controllers/ReminderController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using System.Security.Claims;

namespace FinancialManagementSystem.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReminderController : ControllerBase
    {
        private readonly IReminderService _reminderService;

        public ReminderController(IReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReminders()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var reminders = await _reminderService.GetRemindersAsync(userId);
            return Ok(reminders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReminder(int id)
        {
            var reminder = await _reminderService.GetReminderAsync(id);
            if (reminder == null)
                return NotFound();

            return Ok(reminder);
        }

        [HttpPost]
        public async Task<IActionResult> AddReminder(ReminderModel model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            model.UserId = userId;
            var result = await _reminderService.AddReminderAsync(model);
            return CreatedAtAction(nameof(GetReminder), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReminder(int id, ReminderModel model)
        {
            if (id != model.Id)
                return BadRequest();

            var result = await _reminderService.UpdateReminderAsync(model);
            if (result == null)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReminder(int id)
        {
            await _reminderService.DeleteReminderAsync(id);
            return NoContent();
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingReminders([FromQuery] DateTime endDate)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var reminders = await _reminderService.GetUpcomingRemindersAsync(userId, endDate);
            return Ok(reminders);
        }
    }
}