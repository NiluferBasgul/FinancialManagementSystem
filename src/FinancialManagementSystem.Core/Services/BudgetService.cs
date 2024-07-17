using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;

namespace FinancialManagementSystem.Core.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepository;

        public BudgetService(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public async Task<BudgetModel> CreateBudgetAsync(BudgetModel model)
        {
            var budget = new Budget
            {
                Name = model.Name,
                Amount = model.Amount,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                UserId = model.UserId
            };

            var result = await _budgetRepository.AddAsync(budget);
            return MapToBudgetModel(result);
        }

        public async Task<BudgetModel> GetBudgetAsync(int id)
        {
            var budget = await _budgetRepository.GetByIdAsync(id);
            return budget != null ? MapToBudgetModel(budget) : null;
        }

        public async Task<IEnumerable<BudgetModel>> GetBudgetsAsync(int userId)
        {
            var budgets = await _budgetRepository.GetByUserIdAsync(userId);
            return budgets.Select(MapToBudgetModel);
        }

        public async Task<BudgetModel> UpdateBudgetAsync(BudgetModel model)
        {
            var budget = await _budgetRepository.GetByIdAsync(model.Id);
            if (budget == null)
                return null;

            budget.Name = model.Name;
            budget.Amount = model.Amount;
            budget.StartDate = model.StartDate;
            budget.EndDate = model.EndDate;

            var result = await _budgetRepository.UpdateAsync(budget);
            return MapToBudgetModel(result);
        }

        public async Task DeleteBudgetAsync(int id)
        {
            await _budgetRepository.DeleteAsync(id);
        }

        private BudgetModel MapToBudgetModel(Budget budget)
        {
            return new BudgetModel
            {
                Id = budget.Id,
                Name = budget.Name,
                Amount = budget.Amount,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                UserId = budget.UserId
            };
        }

        public async Task<BudgetResult> AddBudgetAsync(int userId, BudgetModel model)
        {
            model.UserId = userId;
            var budget = new Budget
            {
                Name = model.Name,
                Amount = model.Amount,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                UserId = userId
            };

            try
            {
                var result = await _budgetRepository.AddAsync(budget);
                return new BudgetResult { Succeeded = true, BudgetId = result.Id };
            }
            catch (Exception ex)
            {
                return new BudgetResult { Succeeded = false, Errors = new List<string> { ex.Message } };
            }
        }
    }
}
