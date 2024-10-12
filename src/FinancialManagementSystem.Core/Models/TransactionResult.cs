using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Models
{
    [ExcludeFromCodeCoverage]
    public class TransactionResult
    {
        public bool Succeeded { get; set; }
        public int? TransactionId { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
