namespace FinancialManagementSystem.Core.Models
{
    public class TransactionResult
    {
        public bool Succeeded { get; set; }
        public int? TransactionId { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
