using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinancialManagementSystem.API.Controllers
{
    /// <summary>
    /// Controller responsible for managing user expenses.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;
        private readonly IBudgetService _budgetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpenseController"/> class.
        /// </summary>
        /// <param name="expenseService">Service for managing expenses.</param>
        /// <param name="budgetService">Service for managing budgets.</param>
        public ExpenseController(IExpenseService expenseService, IBudgetService budgetService)
        {
            _expenseService = expenseService;
            _budgetService = budgetService;
        }

        /// <summary>
        /// Retrieves the user's expenses within a specified date range.
        /// </summary>
        /// <param name="from">Start date of the range. Format: yyyy-MM-ddTHH:mm:ssZ. Example: 2023-10-01T00:00:00Z</param>
        /// <param name="to">End date of the range. Format: yyyy-MM-ddTHH:mm:ssZ. Example: 2023-10-31T23:59:59Z</param>
        /// <returns>A list of expenses for the user within the specified date range.</returns>
        /// <remarks>
        /// Sample request:
        /// GET /api/expenses?from=2023-10-01T00:00:00Z&amp;to=2023-10-31T23:59:59Z
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> GetExpenses([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var expenses = await _expenseService.GetExpensesByUserAsync(userId, from, to);
            return Ok(expenses);
        }

        /// <summary>
        /// Adds a new expense for the authenticated user.
        /// </summary>
        /// <param name="model">The expense model containing details of the expense.</param>
        /// <returns>Created expense if successful, otherwise BadRequest with validation errors.</returns>
        [HttpPost]
        public async Task<IActionResult> AddExpense([FromBody] ExpenseModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var expense = await _expenseService.AddExpenseAsync(model, userId);
            return CreatedAtAction(nameof(GetExpenses), new { id = expense.Id }, expense);
        }

        /// <summary>
        /// Updates an existing expense for the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the expense to update.</param>
        /// <param name="model">The updated expense model.</param>
        /// <returns>NoContent if successful, otherwise BadRequest with validation errors.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] ExpenseModel model)
        {
            if (id != model.Id || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _expenseService.UpdateExpenseAsync(model);
            return NoContent();
        }

        /// <summary>
        /// Deletes an expense for the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the expense to delete.</param>
        /// <returns>NoContent if successful.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            await _expenseService.DeleteExpenseAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Retrieves the financial summary for the authenticated user.
        /// </summary>
        /// <returns>A financial summary including total income, total expenses, and total savings.</returns>
        [HttpGet("financial-summary")]
        public IActionResult GetFinancialSummary()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var summary = _expenseService.GetFinancialSummary(userId);
            return Ok(summary);
        }
    }
}
