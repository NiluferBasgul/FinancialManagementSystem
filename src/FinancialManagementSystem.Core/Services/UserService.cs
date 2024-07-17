using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;

namespace FinancialManagementSystem.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserModel> GetUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return null;

            return new UserModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };
        }

        public async Task<UserModel> UpdateUserAsync(UserUpdateModel model)
        {
            var user = await _userRepository.GetByIdAsync(model.Id);
            if (user == null)
                return null;

            user.Username = model.Username;
            user.Email = model.Email;

            var updatedUser = await _userRepository.UpdateAsync(user);
            return new UserModel
            {
                Id = updatedUser.Id,
                Username = updatedUser.Username,
                Email = updatedUser.Email
            };
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            await _userRepository.DeleteAsync(id);
            return true;
        }
    }
}
