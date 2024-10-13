using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Entities
{
    [ExcludeFromCodeCoverage]
    public class Budget
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<BudgetCategory> Needs { get; set; } = new List<BudgetCategory>();
        public ICollection<BudgetCategory> Wants { get; set; } = new List<BudgetCategory>();
        public ICollection<BudgetCategory> Savings { get; set; } = new List<BudgetCategory>();
    }
}
