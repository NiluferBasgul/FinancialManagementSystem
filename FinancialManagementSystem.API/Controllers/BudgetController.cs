using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.API.Models;
using System.Threading.Tasks;
using System.Security.Claims;

namespace FinancialManagementSystem.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBudgets()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var budgets = await _budgetService.GetBudgetsAsync(userId);
            return Ok(budgets);
        }

        [HttpPost]
        public async Task<IActionResult> AddBudget(BudgetModel model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _budgetService.AddBudgetAsync(userId, model);
            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(GetBudgets), new { id = result.BudgetId }, result);
            }
            return BadRequest(result.Errors);
        }

        // Implement UpdateBudget and DeleteBudget methods similar to the TransactionController
    }
}