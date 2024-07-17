using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Models;
using TransactionModel = FinancialManagementSystem.Core.Interfaces.TransactionModel;

namespace FinancialManagementSystem.Core.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsAsync(int userId)
        {
            return await _transactionRepository.GetTransactionsByUserIdAsync(userId);
        }

        public async Task<TransactionResult> AddTransactionAsync(int userId, TransactionModel model)
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
            return new TransactionResult { Succeeded = true, TransactionId = result.Id };
        }

        public async Task<TransactionResult> UpdateTransactionAsync(int userId, int transactionId, TransactionModel model)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null || transaction.UserId != userId)
            {
                return new TransactionResult { Succeeded = false, Errors = new[] { "Transaction not found or unauthorized" } };
            }

            transaction.Amount = model.Amount;
            transaction.Description = model.Description;
            transaction.Date = model.Date;
            transaction.Category = model.Category;
            transaction.Type = model.Type;
            transaction.UpdatedAt = DateTime.UtcNow;

            await _transactionRepository.UpdateAsync(transaction);
            return new TransactionResult { Succeeded = true };
        }

        public async Task<TransactionResult> DeleteTransactionAsync(int userId, int transactionId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null || transaction.UserId != userId)
            {
                return new TransactionResult { Succeeded = false, Errors = new[] { "Transaction not found or unauthorized" } };
            }

            await _transactionRepository.DeleteAsync(transaction);
            return new TransactionResult { Succeeded = true };
        }
    }
}