using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Entities
{
    [ExcludeFromCodeCoverage]
    public class Need
    {
        public int Id { get; set; }

        [Required]
        public int BudgetId { get; set; }

        [Required]
        [StringLength(100)]
        public string Category { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Value must be a positive number")]
        public decimal Value { get; set; }

        // Navigation Property
        public Budget Budget { get; set; }
    }
}
