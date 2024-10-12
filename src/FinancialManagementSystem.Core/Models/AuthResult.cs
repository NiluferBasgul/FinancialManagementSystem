using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Models
{
    [ExcludeFromCodeCoverage]
    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public string Token { get; set; }
        public string ErrorMessage { get; set; }
    }
}
