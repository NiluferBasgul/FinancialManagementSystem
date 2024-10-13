using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialManagementSystem.Core.Models
{
    public class BudgetTotalsModel
    {
        public decimal Needs { get; set; }
        public decimal Wants { get; set; }
        public decimal Savings { get; set; }
        public decimal Income { get; set; }
    }
}
