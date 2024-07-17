using FinancialManagementSystem.Core.Models;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface IUserService
    {
        Task<UserModel> GetUserAsync(int id);
        Task<UserModel> UpdateUserAsync(UserUpdateModel model);
        Task<bool> DeleteUserAsync(int id);
    }
}
