using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Models
{
    [ExcludeFromCodeCoverage]
    public class TransferRequest
    {
        public decimal Amount { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
    }
}
