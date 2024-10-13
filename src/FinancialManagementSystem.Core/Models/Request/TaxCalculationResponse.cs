using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialManagementSystem.Core.Models.Request
{
    public class TaxCalculationResponse
    {
        public decimal UnemploymentInsurance { get; set; }
        public decimal TotalTax { get; set; }
        public decimal PersonalIncomeTax { get; set; }
        public decimal PensionDisabilityInsurance { get; set; }
        public decimal HealthInsurance { get; set; }
    }
}
