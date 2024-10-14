using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Models.Request
{
    [ExcludeFromCodeCoverage]
    public class TaxCalculationResponse
    {
        public decimal UnemploymentInsurance { get; set; }
        public decimal TotalTax { get; set; }
        public decimal PersonalIncomeTax { get; set; }
        public decimal PensionDisabilityInsurance { get; set; }
        public decimal HealthInsurance { get; set; }
    }
}
