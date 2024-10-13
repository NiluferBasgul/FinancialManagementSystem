using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Models
{
    [ExcludeFromCodeCoverage]
    public class IncomeModel
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Tax cannot be longer than 100 characters")]
        public string Tax { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Type cannot be longer than 50 characters")]
        public string Type { get; set; }
    }
}
