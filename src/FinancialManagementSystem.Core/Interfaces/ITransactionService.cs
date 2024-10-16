﻿using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Models;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetTransactionsAsync(int userId, int skip, int take);
        Task<TransactionResult> AddTransactionAsync(int userId, TransactionModel model);
        Task<TransactionResult> UpdateTransactionAsync(int userId, int transactionId, TransactionModel model);
        Task<TransactionResult> DeleteTransactionAsync(int userId, int transactionId);
    }
}