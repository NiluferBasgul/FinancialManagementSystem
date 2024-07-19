// FinancialManagementSystem.Core/Entities/Reminder.cs
namespace FinancialManagementSystem.Core.Entities
{
    public class Reminder
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}