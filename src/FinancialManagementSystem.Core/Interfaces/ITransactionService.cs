﻿using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Models;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetTransactionsAsync(int userId);
        Task<TransactionResult> AddTransactionAsync(int userId, TransactionModel model);
        Task<TransactionResult> UpdateTransactionAsync(int userId, int transactionId, TransactionModel model);
        Task<TransactionResult> DeleteTransactionAsync(int userId, int transactionId);
    }
}