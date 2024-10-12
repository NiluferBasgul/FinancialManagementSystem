using FinancialManagementSystem.Core.Data;
using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagementSystem.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(int userId, int skip, int take)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId && !t.IsDeleted)
                .OrderByDescending(t => t.Date)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<Transaction> GetByIdAsync(int id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task<Transaction> AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Transaction transaction)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
    }
}