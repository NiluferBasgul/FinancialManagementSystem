using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using Microsoft.Extensions.Logging;

namespace FinancialManagementSystem.Core.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<BudgetService> _logger;
        private readonly IAccountRepository _accountRepository;
        private string _currentTab;

        public BudgetService(
            IBudgetRepository budgetRepository,
            IUserRepository userRepository,
            ILogger<BudgetService> logger,
            IAccountRepository accountRepository)
        {
            _budgetRepository = budgetRepository;
            _userRepository = userRepository;
            _logger = logger;
            _accountRepository = accountRepository;
        }

        #region Budget Management

        public async Task<BudgetModel> CreateBudgetAsync(BudgetModel model)
        {
            ValidateBudgetModel(model);

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
                    return null;
                }
                return MapToBudgetModel(budget);
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
            var budget = await _budgetRepository.GetByIdAsync(model.Id);
            if (budget == null)
            {
                _logger.LogWarning($"Budget with id {model.Id} not found for update");
                return null;
            }

            MapToBudgetEntity(model, budget);
            await _budgetRepository.UpdateAsync(budget);

            return MapToBudgetModel(budget);
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
        public void TransferFunds(TransferRequest transferRequest)
        {
            var fromAccount = _accountRepository.GetById(transferRequest.FromAccountId);
            var toAccount = _accountRepository.GetById(transferRequest.ToAccountId);

            _logger.LogInformation($"Transfer attempt: From Account ID: {transferRequest.FromAccountId}, To Account ID: {transferRequest.ToAccountId}, Amount: {transferRequest.Amount}");
            _logger.LogInformation($"From Account Balance: {fromAccount?.Balance}, To Account Balance: {toAccount?.Balance}");

            if (fromAccount == null || toAccount == null)
            {
                _logger.LogWarning("One or both accounts not found.");
                throw new InvalidOperationException("One or both accounts not found.");
            }

            if (fromAccount.Balance >= transferRequest.Amount)
            {
                fromAccount.Balance -= transferRequest.Amount;
                toAccount.Balance += transferRequest.Amount;
                _accountRepository.Update(fromAccount);
                _accountRepository.Update(toAccount);
                _logger.LogInformation($"Transfer successful. New balances - From: {fromAccount.Balance}, To: {toAccount.Balance}");
            }
            else
            {
                _logger.LogWarning($"Insufficient funds. Required: {transferRequest.Amount}, Available: {fromAccount.Balance}");
                throw new InvalidOperationException("Insufficient funds.");
            }
        }
        public async Task<BudgetResult> AddBudgetAsync(int userId, BudgetModel model)
        {
            ValidateBudgetModel(model);
            model.UserId = userId;

            try
            {
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

        #endregion

        #region Needs, Wants, and Savings
        public async Task<IEnumerable<BudgetCategoryModel>> GetNeedsBudgetAsync(int userId)
        {
            var budgets = await _budgetRepository.GetByUserIdAsync(userId);
            if (budgets == null || !budgets.Any())
            {
                _logger.LogWarning($"No budget found for user {userId}");
                return new List<BudgetCategoryModel>(); 
            }

            return budgets.SelectMany(b => b.Needs)
                          .Select(n => new BudgetCategoryModel { Category = n.Category, Value = n.Value });
        }

        public async Task<IEnumerable<BudgetCategoryModel>> GetWantsBudgetAsync(int userId)
        {
            return await GetBudgetCategoriesAsync(userId, "Wants");
        }

        public async Task<bool> UpdateNeedsAmount(int userId, List<BudgetCategoryModel> needs)
        {
            var budget = (await _budgetRepository.GetByUserIdAsync(userId)).FirstOrDefault();

            if (budget == null)
            {
                _logger.LogInformation($"No budget found for user {userId}. Creating a new budget.");

                budget = new Budget
                {
                    UserId = userId,
                    Name = "Default Budget",
                    Amount = needs.Sum(n => n.Value) ?? 0,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(1),
                    Needs = new List<BudgetCategory>(),
                    Savings = new List<BudgetCategory>(),
                    Wants = new List<BudgetCategory>()
                };

                await _budgetRepository.AddAsync(budget);
            }

            budget.Needs.Clear();
            foreach (var need in needs)
            {
                budget.Needs.Add(new BudgetCategory
                {
                    Category = need.Category,
                    Value = need.Value,
                    NeedsBudgetId = budget.Id, 
                });
            }

            await _budgetRepository.UpdateAsync(budget);
            return true;
        }

        public async Task UpdateWantsAmount(int userId, List<BudgetCategoryModel> wants)
        {
            var budget = (await _budgetRepository.GetByUserIdAsync(userId)).FirstOrDefault();

            if (budget == null)
            {
                _logger.LogInformation($"No budget found for user {userId}. Creating a new budget.");

                budget = new Budget
                {
                    UserId = userId,
                    Name = "Default Budget",
                    Amount = wants.Sum(w => w.Value) ?? 0,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(1),
                    Wants = new List<BudgetCategory>()
                };

                await _budgetRepository.AddAsync(budget);
            }

            budget.Wants.Clear();
            foreach (var want in wants)
            {
                budget.Wants.Add(new BudgetCategory
                {
                    Category = want.Category,
                    Value = want.Value,
                    WantsBudgetId = budget.Id, 
                    NeedsBudgetId = null,  
                    SavingsBudgetId = null    
                });
            }

            await _budgetRepository.UpdateAsync(budget);
        }

        private async Task<IEnumerable<BudgetCategoryModel>> GetBudgetCategoriesAsync(int userId, string categoryType)
        {
            var budgets = await _budgetRepository.GetByUserIdAsync(userId);
            return categoryType switch
            {
                "Needs" => budgets.SelectMany(b => b.Needs).Select(MapToBudgetCategoryModel),
                "Wants" => budgets.SelectMany(b => b.Wants).Select(MapToBudgetCategoryModel),
                _ => throw new ArgumentException("Invalid category type")
            };
        }


        private async Task UpdateBudgetCategories(int budgetId, List<BudgetCategoryModel> categories, string categoryType)
        {
            var budget = await _budgetRepository.GetByIdAsync(budgetId);
            if (budget == null)
            {
                _logger.LogWarning($"Budget with id {budgetId} not found for update");
                return;
            }

            switch (categoryType)
            {
                case "Needs":
                    UpdateCategoryList(budget.Needs, categories);
                    break;
                case "Wants":
                    UpdateCategoryList(budget.Wants, categories);
                    break;
                default:
                    throw new ArgumentException("Invalid category type");
            }

            await _budgetRepository.UpdateAsync(budget);
        }

        private void UpdateCategoryList(ICollection<BudgetCategory> existingCategories, List<BudgetCategoryModel> newCategories)
        {
            existingCategories.Clear();
            foreach (var category in newCategories)
            {
                existingCategories.Add(new BudgetCategory
                {
                    Category = category.Category,
                    Value = category.Value
                });
            }
        }

        #endregion

        #region Budget Totals

        public async Task<BudgetTotalsModel> GetBudgetTotalsAsync(int userId)
        {
            var budgets = await _budgetRepository.GetByUserIdAsync(userId);
            var needs = budgets.Sum(b => b.Needs.Sum(n => n.Value));
            var wants = budgets.Sum(b => b.Wants.Sum(w => w.Value));

            return new BudgetTotalsModel
            {
                Needs = needs,
                Wants = wants,
                Income = 10000
            };
        }

        #endregion

        #region Utility Methods

        private void ValidateBudgetModel(BudgetModel model)
        {
            if (model.UserId <= 0)
            {
                throw new ArgumentException("Invalid user ID");
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentException("Budget name cannot be empty");
            }

            if (model.Amount <= 0)
            {
                throw new ArgumentException("Budget amount must be greater than zero");
            }

            if (model.StartDate >= model.EndDate)
            {
                throw new ArgumentException("End date must be later than start date");
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
                UserId = budget.UserId,
                Needs = budget.Needs.Select(MapToBudgetCategoryModel).ToList(),
                Wants = budget.Wants.Select(MapToBudgetCategoryModel).ToList(),
            };
        }

        private BudgetCategoryModel MapToBudgetCategoryModel(BudgetCategory category)
        {
            return new BudgetCategoryModel
            {
                Category = category.Category,
                Value = category.Value
            };
        }

        private void MapToBudgetEntity(BudgetModel model, Budget budget)
        {
            budget.Name = model.Name;
            budget.Amount = model.Amount;
            budget.StartDate = model.StartDate;
            budget.EndDate = model.EndDate;

            UpdateCategoryList(budget.Needs, model.Needs);
            UpdateCategoryList(budget.Wants, model.Wants);
        }

        #endregion
    }
}
