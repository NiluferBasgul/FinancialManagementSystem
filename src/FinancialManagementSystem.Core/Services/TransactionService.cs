using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Models;
using Microsoft.Extensions.Logging;

namespace FinancialManagementSystem.Core.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(ITransactionRepository transactionRepository, ILogger<TransactionService> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsAsync(int userId)
        {
            try
            {
                var transactions = await _transactionRepository.GetTransactionsByUserIdAsync(userId);
                _logger.LogInformation($"Retrieved {transactions.Count()} transactions for user {userId}");
                return transactions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving transactions for user {userId}");
                throw;
            }
        }

        public async Task<TransactionResult> AddTransactionAsync(int userId, TransactionModel model)
        {
            try
            {
                var transaction = new Transaction
                {
                    UserId = userId,
                    Amount = model.Amount,
                    Description = model.Description,
                    Date = model.Date,
                    Category = model.Category,
                    Type = model.Type,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var result = await _transactionRepository.AddAsync(transaction);
                _logger.LogInformation($"Transaction added for user {userId}: {model.Amount:C2} - {model.Description}");
                return new TransactionResult { Succeeded = true, TransactionId = result.Id };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding transaction for user {userId}");
return new TransactionResult { Succeeded = false, Errors = new List<string> { ex.Message } };

            }
        }

        public async Task<TransactionResult> UpdateTransactionAsync(int userId, int transactionId, TransactionModel model)
        {
            try
            {
                var transaction = await _transactionRepository.GetByIdAsync(transactionId);
                if (transaction == null || transaction.UserId != userId)
                {
                    _logger.LogWarning($"Transaction not found or unauthorized: ID {transactionId}, User {userId}");
                    return new TransactionResult { Succeeded = false, Errors = new List<string> { "Transaction not found or unauthorized" } };
                }

                transaction.Amount = model.Amount;
                transaction.Description = model.Description;
                transaction.Date = model.Date;
                transaction.Category = model.Category;
                transaction.Type = model.Type;
                transaction.UpdatedAt = DateTime.UtcNow;

                await _transactionRepository.UpdateAsync(transaction);
                _logger.LogInformation($"Transaction updated: ID {transactionId}, User {userId}");
                return new TransactionResult { Succeeded = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating transaction: ID {transactionId}, User {userId}");
                return new TransactionResult { Succeeded = false, Errors = new List<string> { ex.Message } };
            }
        }

        public async Task<TransactionResult> DeleteTransactionAsync(int userId, int transactionId)
        {
            try
            {
                var transaction = await _transactionRepository.GetByIdAsync(transactionId);
                if (transaction == null || transaction.UserId != userId)
                {
                    _logger.LogWarning($"Transaction not found or unauthorized for deletion: ID {transactionId}, User {userId}");
                    return new TransactionResult { Succeeded = false, Errors = new List<string> { "Transaction not found or unauthorized" } };
                }

                await _transactionRepository.DeleteAsync(transaction);
                _logger.LogInformation($"Transaction deleted: ID {transactionId}, User {userId}");
                return new TransactionResult { Succeeded = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting transaction: ID {transactionId}, User {userId}");
                return new TransactionResult { Succeeded = false, Errors = new List<string> { ex.Message } };

            }
        }
    }
}