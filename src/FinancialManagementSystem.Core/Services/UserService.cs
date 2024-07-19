using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using Microsoft.Extensions.Logging;

namespace FinancialManagementSystem.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserModel> GetUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"User with id {id} not found");
                    return null;
                }

                _logger.LogInformation($"Retrieved user: ID {id}");
                return new UserModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user with id {id}");
                throw;
            }
        }

        public async Task<UserModel> UpdateUserAsync(UserUpdateModel model)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(model.Id);
                if (user == null)
                {
                    _logger.LogWarning($"User with id {model.Id} not found for update");
                    return null;
                }

                user.Username = model.Username;
                user.Email = model.Email;

                var updatedUser = await _userRepository.UpdateAsync(user);
                _logger.LogInformation($"User updated: ID {updatedUser.Id}");
                return new UserModel
                {
                    Id = updatedUser.Id,
                    Username = updatedUser.Username,
                    Email = updatedUser.Email
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user with id {model.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"User with id {id} not found for deletion");
                    return false;
                }

                await _userRepository.DeleteAsync(id);
                _logger.LogInformation($"User deleted: ID {id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user with id {id}");
                throw;
            }
        }

        public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserModel
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email
            });
        }
    }
}