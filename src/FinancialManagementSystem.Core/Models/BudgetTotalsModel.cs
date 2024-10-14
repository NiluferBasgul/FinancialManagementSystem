using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Models
{
    [ExcludeFromCodeCoverage]
    public class BudgetTotalsModel
    {
        public decimal? Needs { get; set; }
        public decimal? Wants { get; set; }
        public decimal Savings { get; set; }
        public decimal Income { get; set; }
    }
}
