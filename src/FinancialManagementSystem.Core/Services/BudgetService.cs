using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using Microsoft.Extensions.Logging;

namespace FinancialManagementSystem.Core.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly ILogger<BudgetService> _logger;


        public BudgetService(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public async Task<BudgetModel> CreateBudgetAsync(BudgetModel model)
        {
            try
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
                _logger.LogInformation($"Budget created for user {model.UserId}: {model.Name}");
                return MapToBudgetModel(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating budget for user {model.UserId}");
                throw;
            }
        }

        public async Task<BudgetModel> GetBudgetAsync(int id)
        {
            try
            {
                var budget = await _budgetRepository.GetByIdAsync(id);
                if (budget == null)
                {
                    _logger.LogWarning($"Budget with id {id} not found");
                }
                return budget != null ? MapToBudgetModel(budget) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving budget with id {id}");
                throw;
            }
        }

        public async Task<IEnumerable<BudgetModel>> GetBudgetsAsync(int userId)
        {
            try
            {
                var budgets = await _budgetRepository.GetByUserIdAsync(userId);
                _logger.LogInformation($"Retrieved {budgets.Count()} budgets for user {userId}");
                return budgets.Select(MapToBudgetModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving budgets for user {userId}");
                throw;
            }
        }

        public async Task<BudgetModel> UpdateBudgetAsync(BudgetModel model)
        {
            try
            {
                var budget = await _budgetRepository.GetByIdAsync(model.Id);
                if (budget == null)
                {
                    _logger.LogWarning($"Budget with id {model.Id} not found for update");
                    return null;
                }

                budget.Name = model.Name;
                budget.Amount = model.Amount;
                budget.StartDate = model.StartDate;
                budget.EndDate = model.EndDate;

                var result = await _budgetRepository.UpdateAsync(budget);
                _logger.LogInformation($"Budget updated: ID {model.Id}, User {budget.UserId}");
                return MapToBudgetModel(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating budget with id {model.Id}");
                throw;
            }
        }

        public async Task DeleteBudgetAsync(int id)
        {
            try
            {
                await _budgetRepository.DeleteAsync(id);
                _logger.LogInformation($"Budget deleted: ID {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting budget with id {id}");
                throw;
            }
        }

        public async Task<BudgetResult> AddBudgetAsync(int userId, BudgetModel model)
        {
            try
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

                var result = await _budgetRepository.AddAsync(budget);
                _logger.LogInformation($"Budget added for user {userId}: {model.Name}");
                return new BudgetResult { Succeeded = true, BudgetId = result.Id };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding budget for user {userId}");
                return new BudgetResult { Succeeded = false, Errors = new List<string> { ex.Message } };
            }
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
    }
}
