using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Models
{
    [ExcludeFromCodeCoverage]
    public class BudgetModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int UserId { get; set; }

        // Collections for Needs, Wants, and Savings
        public List<BudgetCategoryModel> Needs { get; set; } = new List<BudgetCategoryModel>();
        public List<BudgetCategoryModel> Wants { get; set; } = new List<BudgetCategoryModel>();
        public List<BudgetCategoryModel> Savings { get; set; } = new List<BudgetCategoryModel>();
    }
}
