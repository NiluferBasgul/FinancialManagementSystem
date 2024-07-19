// FinancialManagementSystem.API/Controllers/IncomeController.cs
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
    public class IncomeController : ControllerBase
    {
        private readonly IIncomeService _incomeService;

        public IncomeController(IIncomeService incomeService)
        {
            _incomeService = incomeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetIncomes()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var incomes = await _incomeService.GetIncomesAsync(userId);
            return Ok(incomes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIncome(int id)
        {
            var income = await _incomeService.GetIncomeAsync(id);
            if (income == null)
                return NotFound();

            return Ok(income);
        }

        [HttpPost]
        public async Task<IActionResult> AddIncome(IncomeModel model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            model.UserId = userId;
            var result = await _incomeService.AddIncomeAsync(model);
            return CreatedAtAction(nameof(GetIncome), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateIncome(int id, IncomeModel model)
        {
            if (id != model.Id)
                return BadRequest();

            var result = await _incomeService.UpdateIncomeAsync(model);
            if (result == null)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncome(int id)
        {
            await _incomeService.DeleteIncomeAsync(id);
            return NoContent();
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetTotalIncomeForPeriod([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var total = await _incomeService.GetTotalIncomeForPeriodAsync(userId, startDate, endDate);
            return Ok(new { TotalIncome = total });
        }
    }
}