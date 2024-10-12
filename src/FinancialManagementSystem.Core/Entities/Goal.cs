using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Entities
{
    [ExcludeFromCodeCoverage]
    public class Goal
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime TargetDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}
