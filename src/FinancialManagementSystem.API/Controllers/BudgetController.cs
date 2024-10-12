using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinancialManagementSystem.API.Controllers
{
    /// <summary>
    /// Controller responsible for managing user budgets.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _budgetService;
        private readonly ILogger<BudgetController> _logger;

        /// <summary>
        /// Constructor that injects the budget service and logger.
        /// </summary>
        /// <param name="budgetService">Service for handling budget operations.</param>
        /// <param name="logger">Logger instance.</param>
        public BudgetController(IBudgetService budgetService, ILogger<BudgetController> logger)
        {
            _budgetService = budgetService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the budgets for the authenticated user.
        /// </summary>
        /// <returns>A list of user budgets.</returns>
        [Authorize]
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

        /// <summary>
        /// Adds a new budget for the authenticated user.
        /// </summary>
        /// <param name="model">Budget model containing budget details like name and amount.</param>
        /// <returns>Created budget if successful, otherwise BadRequest with validation errors.</returns>
        [Authorize]
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

        /// <summary>
        /// Deletes a budget by its ID for the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the budget to delete.</param>
        /// <returns>NoContent if successful, otherwise BadRequest with an error message.</returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                await _budgetService.DeleteBudgetAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting budget");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves the current active tab for the budget management interface.
        /// </summary>
        /// <returns>Current active tab.</returns>
        [Authorize]
        [HttpGet("currentTab")]
        public IActionResult GetCurrentTab()
        {
            var currentTab = _budgetService.GetCurrentTab();
            return Ok(new { currentTab });
        }

        /// <summary>
        /// Sets the current active tab in the budget management interface.
        /// </summary>
        /// <param name="tabName">Name of the tab to set as active.</param>
        /// <returns>Ok if successful.</returns>
        [Authorize]
        [HttpPost("setTab")]
        public IActionResult SetTab([FromBody] string tabName)
        {
            _budgetService.SetTab(tabName);
            return Ok();
        }

        /// <summary>
        /// Retrieves the needs budget for the authenticated user.
        /// </summary>
        /// <returns>The needs portion of the budget.</returns>
        [Authorize]
        [HttpGet("needs")]
        public IActionResult GetNeedsBudget()
        {
            var needsBudget = _budgetService.GetNeedsBudget();
            return Ok(new { needsBudget });
        }

        /// <summary>
        /// Submits the needs portion of the budget for a specific budget ID.
        /// </summary>
        /// <param name="budgetId">The ID of the budget.</param>
        /// <param name="needsAmount">The amount allocated to needs.</param>
        /// <returns>Ok if successful.</returns>
        [Authorize]
        [HttpPost("submitNeeds")]
        public async Task<IActionResult> SubmitNeeds(int budgetId, decimal needsAmount)
        {
            await _budgetService.UpdateNeedsAmount(budgetId, needsAmount);
            return Ok();
        }

        /// <summary>
        /// Retrieves the needs portion of a specific budget by ID.
        /// </summary>
        /// <param name="id">The ID of the budget.</param>
        /// <returns>The needs portion of the specified budget.</returns>
        [Authorize]
        [HttpGet("{id}/needs")]
        public async Task<IActionResult> GetNeedsBudget(int id)
        {
            var budget = await _budgetService.GetBudgetAsync(id);
            if (budget == null)
            {
                return NotFound("Budget not found");
            }

            return Ok(new { Needs = budget.Needs });
        }

        /// <summary>
        /// Transfers funds between accounts in the budget.
        /// </summary>
        /// <param name="transferRequest">Transfer request model containing details of the transfer.</param>
        /// <returns>Ok if successful, otherwise BadRequest with an error message.</returns>
        [Authorize]
        [HttpPost("transfer")]
        public IActionResult TransferFunds([FromBody] TransferRequest transferRequest)
        {
            _budgetService.TransferFunds(transferRequest);
            return Ok(new { message = "Transfer successful" });
        }
    }
}
