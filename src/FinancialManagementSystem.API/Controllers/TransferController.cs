using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinancialManagementSystem.API.Controllers
{
    /// <summary>
    /// Controller responsible for handling transfers between accounts or budget categories.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransferController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        /// <summary>
        /// Constructor that injects the budget service.
        /// </summary>
        /// <param name="budgetService">Budget service interface.</param>
        public TransferController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        /// <summary>
        /// Transfers funds between accounts or budget categories.
        /// </summary>
        /// <param name="transferRequest">The transfer request containing the source and destination accounts, and the amount to transfer.</param>
        /// <returns>An Ok response if the transfer is successful, otherwise BadRequest.</returns>
        [HttpPost]
        public IActionResult TransferFunds([FromBody] TransferRequest transferRequest)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (transferRequest.FromAccountId == transferRequest.ToAccountId)
            {
                return BadRequest("Cannot transfer to the same account.");
            }

            try
            {
                _budgetService.TransferFunds(transferRequest);
                return Ok(new { message = "Transfer successful" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
