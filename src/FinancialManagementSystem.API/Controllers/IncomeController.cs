using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinancialManagementSystem.API.Controllers
{
    /// <summary>
    /// Controller responsible for managing user incomes.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class IncomeController : ControllerBase
    {
        private readonly IIncomeService _incomeService;

        /// <summary>
        /// Constructor that injects the income service.
        /// </summary>
        /// <param name="incomeService">Income service interface.</param>
        public IncomeController(IIncomeService incomeService)
        {
            _incomeService = incomeService;
        }

        /// <summary>
        /// Retrieves all incomes for the authenticated user.
        /// </summary>
        /// <returns>A list of incomes.</returns>
        [HttpGet]
        public async Task<IActionResult> GetIncomes()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized("User not authenticated");
                }

                var userId = int.Parse(userIdClaim.Value);
                var incomes = await _incomeService.GetIncomesAsync(userId);
                return Ok(incomes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a specific income by ID.
        /// </summary>
        /// <param name="id">The ID of the income to retrieve.</param>
        /// <returns>The income details if found, otherwise NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIncome(int id)
        {
            var income = await _incomeService.GetIncomeAsync(id);
            if (income == null)
                return NotFound("Income not found");

            return Ok(income);
        }

        /// <summary>
        /// Adds a new income for the authenticated user.
        /// </summary>
        /// <param name="model">Income model containing income details like amount and date.</param>
        /// <returns>The created income if successful, otherwise BadRequest with errors.</returns>
        [HttpPost]
        public async Task<IActionResult> AddIncome(IncomeModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            model.UserId = userId;
            var result = await _incomeService.AddIncomeAsync(model);
            return CreatedAtAction(nameof(GetIncome), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates an existing income by ID.
        /// </summary>
        /// <param name="id">The ID of the income to update.</param>
        /// <param name="model">Income model with updated details.</param>
        /// <returns>NoContent if successful, otherwise BadRequest or NotFound.</returns>
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

        /// <summary>
        /// Deletes an existing income by ID.
        /// </summary>
        /// <param name="id">The ID of the income to delete.</param>
        /// <returns>NoContent if successful, otherwise NotFound.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncome(int id)
        {
            await _incomeService.DeleteIncomeAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Retrieves the total income for a specified period.
        /// </summary>
        /// <param name="startDate">The start date of the period.</param>
        /// <param name="endDate">The end date of the period.</param>
        /// <returns>The total income for the specified period.</returns>
        [HttpGet("total")]
        public async Task<IActionResult> GetTotalIncomeForPeriod([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest("Start date cannot be later than end date.");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var total = await _incomeService.GetTotalIncomeForPeriodAsync(userId, startDate, endDate);
            return Ok(new { TotalIncome = total });
        }
    }
}
