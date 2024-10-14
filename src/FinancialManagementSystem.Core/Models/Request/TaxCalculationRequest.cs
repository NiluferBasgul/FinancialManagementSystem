using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Models.Request
{
    [ExcludeFromCodeCoverage]
    public class TaxCalculationRequest
    {
        public decimal Income { get; set; }
        public decimal Deductions { get; set; }
        public string FilingStatus { get; set; }
    }
}
