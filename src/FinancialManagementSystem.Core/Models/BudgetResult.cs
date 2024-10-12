using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Models
{
    [ExcludeFromCodeCoverage]
    public class BudgetResult
    {
        public bool Succeeded { get; set; }
        public int? BudgetId { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
