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

        public BudgetController(IBudgetService budgetService, ILogger<BudgetController> logger)
        {
            _budgetService = budgetService;
            _logger = logger;
        }

        #region Helper Methods

        private int? GetAuthenticatedUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }

            return null;
        }

        #endregion

        #region Budget Management

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetBudgets()
        {
            var userId = GetAuthenticatedUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var budgets = await _budgetService.GetBudgetsAsync(userId.Value);
                return Ok(budgets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving budgets");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddBudget([FromBody] BudgetModel model)
        {
            var userId = GetAuthenticatedUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var result = await _budgetService.AddBudgetAsync(userId.Value, model);
                if (result.Succeeded)
                {
                    return CreatedAtAction(nameof(GetBudgets), new { id = result.BudgetId }, result);
                }
                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding budget");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            var userId = GetAuthenticatedUserId();
            if (userId == null) return Unauthorized();

            try
            {
                await _budgetService.DeleteBudgetAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting budget");
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region Needs, Wants, and Savings

        [Authorize]
        [HttpGet("needs")]
        public async Task<IActionResult> GetNeedsBudget()
        {
            return await GetBudgetCategories("needs", async (userId) => await _budgetService.GetNeedsBudgetAsync(userId));
        }

        [Authorize]
        [HttpGet("wants")]
        public async Task<IActionResult> GetWantsBudget()
        {
            return await GetBudgetCategories("wants", async (userId) => await _budgetService.GetWantsBudgetAsync(userId));
        }

        [Authorize]
        [HttpPost("submitNeeds")]
        public async Task<IActionResult> SubmitNeeds([FromBody] List<BudgetCategoryModel> needs)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            if (needs == null || !needs.Any())
            {
                return BadRequest("Needs data is required.");
            }

            // Calculate total spent on needs
            decimal totalSpentOnNeeds = needs.Sum(n => n.Value);

            // Update the needs for the user
            var result = await _budgetService.UpdateNeedsAmount(userId, needs);

            if (result)
            {
                return Ok(new { totalSpentOnNeeds });
            }

            return BadRequest("Failed to submit needs");
        }


        [Authorize]
        [HttpPost("submitWants")]
        public async Task<IActionResult> SubmitWants([FromBody] List<BudgetCategoryModel> wants)
        {
            return await SubmitBudgetCategories("wants", wants, async (userId) => await _budgetService.UpdateWantsAmount(userId, wants));
        }

        private async Task<IActionResult> GetBudgetCategories(string categoryType, Func<int, Task<IEnumerable<BudgetCategoryModel>>> fetchCategories)
        {
            var userId = GetAuthenticatedUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var categories = await fetchCategories(userId.Value);
                return Ok(new { categoryType, categories });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving {categoryType} budget");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<IActionResult> SubmitBudgetCategories(string categoryType, List<BudgetCategoryModel> categories, Func<int, Task> submitCategories)
        {
            var userId = GetAuthenticatedUserId();
            if (userId == null) return Unauthorized();

            try
            {
                await submitCategories(userId.Value);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error submitting {categoryType} budget");
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion

        #region Budget Totals

        [Authorize]
        [HttpGet("totals")]
        public async Task<IActionResult> GetBudgetTotals()
        {
            var userId = GetAuthenticatedUserId();
            if (userId == null) return Unauthorized();

            try
            {
                var totals = await _budgetService.GetBudgetTotalsAsync(userId.Value);
                return Ok(totals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving budget totals");
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion

        #region Tab Management
        #endregion

        #region Fund Transfers

        [Authorize]
        [HttpPost("transfer")]
        public IActionResult TransferFunds([FromBody] TransferRequest transferRequest)
        {
            try
            {
                _budgetService.TransferFunds(transferRequest);
                return Ok(new { message = "Transfer successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error transferring funds");
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion
    }
}
