using System.Collections.Generic;
using System.Threading.Tasks;
using FinancialManagementSystem.Core.Entities;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(int userId, int skip, int take);
        Task<Transaction> GetByIdAsync(int id);
        Task<Transaction> AddAsync(Transaction transaction);
        Task UpdateAsync(Transaction transaction);
        Task DeleteAsync(Transaction transaction);
    }
}