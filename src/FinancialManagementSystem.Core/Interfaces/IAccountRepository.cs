using FinancialManagementSystem.Core.Entities;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface IAccountRepository
    {
        Account GetById(int id);
        void Update(Account account);
    }
}
