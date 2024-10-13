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
        void TransferFunds(TransferRequest transferRequest);
        Task<bool> UpdateNeedsAmount(int userId, List<BudgetCategoryModel> needs);
        Task<IEnumerable<BudgetCategoryModel>> GetNeedsBudgetAsync(int userId);
        Task<IEnumerable<BudgetCategoryModel>> GetWantsBudgetAsync(int userId);
        Task<BudgetTotalsModel> GetBudgetTotalsAsync(int userId);
        Task UpdateWantsAmount(int budgetId, List<BudgetCategoryModel> wants);


    }
}
