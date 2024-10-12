using FinancialManagementSystem.Core.Data;
using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;

namespace FinancialManagementSystem.Core.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Account GetById(int id)
        {
            return _context.Accounts.FirstOrDefault(a => a.Id == id);
        }

        public void Update(Account account)
        {
            _context.Update(account);
            _context.SaveChanges();
        }
    }
}
