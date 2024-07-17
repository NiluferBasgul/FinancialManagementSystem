using System.Threading.Tasks;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(string username, string password);
        Task<AuthResult> RegisterAsync(string username, string email, string password);
    }

    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public string? Token { get; set; }
        public string[]? Errors { get; set; }
    }
}