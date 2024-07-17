using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FinancialManagementSystem.Core.Interfaces;
using System.Threading.Tasks;
using System.Security.Claims;
using FinancialManagementSystem.Core.Models;

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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdClaim.Value);
            var budgets = await _budgetService.GetBudgetsAsync(userId);
            return Ok(budgets);
        }

        [HttpPost]
        public async Task<IActionResult> AddBudget(BudgetModel model)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdClaim.Value);
            var result = await _budgetService.AddBudgetAsync(userId, model);
            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(GetBudgets), new { id = result.BudgetId }, result);
            }
            return BadRequest(result.Errors);
        }
    }
}