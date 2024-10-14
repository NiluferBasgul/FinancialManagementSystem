using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Entities
{
    [ExcludeFromCodeCoverage]
    public class BudgetCategory
    {
        public int Id { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal? Value { get; set; }

        [ForeignKey("NeedsBudget")]
        public int? NeedsBudgetId { get; set; }
        public Budget NeedsBudget { get; set; }

        [ForeignKey("WantsBudget")]
        public int? WantsBudgetId { get; set; }
        public Budget WantsBudget { get; set; }

        [ForeignKey("SavingsBudget")]
        public int? SavingsBudgetId { get; set; }
        public Budget SavingsBudget { get; set; }
    }

}
