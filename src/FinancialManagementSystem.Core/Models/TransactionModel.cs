using System.ComponentModel.DataAnnotations;
using FinancialManagementSystem.Core.Entities;

namespace FinancialManagementSystem.Core.Models
{
    public class TransactionModel
    {

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Description cannot be longer than 200 characters")]
        public string? Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Category cannot be longer than 50 characters")]
        public string? Category { get; set; }

        [Required]
        public TransactionType Type { get; set; }
    }
}
