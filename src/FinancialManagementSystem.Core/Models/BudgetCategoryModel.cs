using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Models
{
    [ExcludeFromCodeCoverage]
    public class BudgetCategoryModel
    {
        public string Category { get; set; }
        public decimal? Value { get; set; }
    }
}
