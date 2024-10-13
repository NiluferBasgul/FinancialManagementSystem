using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Models
{
    [ExcludeFromCodeCoverage]
    public class ExpenseModel
    {
        public int Id { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(100, ErrorMessage = "Description cannot exceed 100 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
        public string Category { get; set; }


        [Required(ErrorMessage = "Pay is required")]
        [StringLength(50, ErrorMessage = "Pay cannot exceed 50 characters")]
        public string Pay { get; set; }
    }
}