using FinancialManagementSystem.Core.Models;

namespace FinancialManagementSystem.Core.Interfaces
{
    public interface IBudgetService
    {
        Task<BudgetModel> CreateBudgetAsync(BudgetModel model);
        Task<BudgetModel> GetBudgetAsync(int id);
        Task<IEnumerable<BudgetModel>> GetBudgetsAsync(int userId);
        Task<BudgetModel> UpdateBudgetAsync(BudgetModel model);
        Task DeleteBudgetAsync(int id);
        Task<BudgetResult> AddBudgetAsync(int userId, BudgetModel model);
        string GetCurrentTab();
        void SetTab(string tabName);
        decimal GetNeedsBudget();
        Task<bool> SubmitNeedsAsync(decimal needsAmount, int userId);
        void TransferFunds(TransferRequest transferRequest);
        Task UpdateNeedsAmount(int budgetId, decimal needsAmount);

    }
}
