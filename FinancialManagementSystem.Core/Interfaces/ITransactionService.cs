using System.Collections.Generic;
using System.Threading.Tasks;
using FinancialManagementSystem.Core.Entities;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetTransactionsAsync(int userId);
        Task<TransactionResult> AddTransactionAsync(int userId, TransactionModel model);
        Task<TransactionResult> UpdateTransactionAsync(int userId, int transactionId, TransactionModel model);
        Task<TransactionResult> DeleteTransactionAsync(int userId, int transactionId);
    }

    public class TransactionResult
    {
        public bool Succeeded { get; set; }
        public int? TransactionId { get; set; }
        public string[] Errors { get; set; }
    }

    public class TransactionModel
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public TransactionType Type { get; set; }
    }
}