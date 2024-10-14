using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Models
{
    [ExcludeFromCodeCoverage]
    public class TaxRequest
    {
        public double Income { get; set; }
        public double Deductions { get; set; }
        public string FilingStatus { get; set; }
    }
}
