using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialManagementSystem.Core.Models.Request
{
    public class TaxCalculationRequest
    {
        public decimal Income { get; set; }
        public decimal Deductions { get; set; }
        public string FilingStatus { get; set; }
    }
}
