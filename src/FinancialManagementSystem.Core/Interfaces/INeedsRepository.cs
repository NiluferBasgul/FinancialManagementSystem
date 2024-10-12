using FinancialManagementSystem.Core.Models;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface INeedsRepository
    {
        void SaveNeedsBudget(decimal needsAmount, int userId);
    }
}
