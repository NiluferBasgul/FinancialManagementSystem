using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinancialManagementSystem.API.Controllers
{
    /// <summary>
    /// Controller responsible for managing user transactions.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        /// <summary>
        /// Constructor that injects the transaction service.
        /// </summary>
        /// <param name="transactionService">Transaction service interface.</param>
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// Retrieves all transactions for the authenticated user.
        /// </summary>
        /// <param name="skip">Number of transactions to skip.</param>
        /// <param name="take">Number of transactions to retrieve.</param>
        /// <returns>A list of transactions.</returns>
        [HttpGet]
        public async Task<IActionResult> GetTransactions([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var transactions = await _transactionService.GetTransactionsAsync(userId, skip, take);
            return Ok(transactions);
        }

        /// <summary>
        /// Adds a new transaction for the authenticated user.
        /// </summary>
        /// <param name="model">Transaction model containing transaction details like amount, date, and description.</param>
        /// <returns>The created transaction if successful, otherwise BadRequest with errors.</returns>
        [HttpPost]
        public async Task<IActionResult> AddTransaction([FromBody] TransactionModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _transactionService.AddTransactionAsync(userId, model);
            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(GetTransactions), new { id = result.TransactionId }, result);
            }
            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Updates an existing transaction by ID.
        /// </summary>
        /// <param name="id">The ID of the transaction to update.</param>
        /// <param name="model">Transaction model with updated details.</param>
        /// <returns>NoContent if successful, otherwise BadRequest or NotFound.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, [FromBody] TransactionModel model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _transactionService.UpdateTransactionAsync(userId, id, model);
            if (result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Deletes an existing transaction by ID.
        /// </summary>
        /// <param name="id">The ID of the transaction to delete.</param>
        /// <returns>NoContent if successful, otherwise BadRequest.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _transactionService.DeleteTransactionAsync(userId, id);
            if (result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(result.Errors);
        }
    }
}
