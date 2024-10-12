using FinancialManagementSystem.Core.Entities;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
        User GetUserById(int userId);
        void UpdateAccount(User user);
    }
}