using FinancialManagementSystem.Core.Models;
using FinancialManagementSystem.Core.Models.Response;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(LoginModel model);
        Task<AuthResult> RegisterAsync(RegisterModel model);
        int? ValidateToken(string token);
    }
}