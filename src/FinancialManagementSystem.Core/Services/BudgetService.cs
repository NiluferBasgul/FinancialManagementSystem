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
        private decimal _needsBudget;

        public BudgetService(IBudgetRepository budgetRepository, IUserRepository userRepository, ILogger<BudgetService> logger, IAccountRepository accountRepository)
        {
            _budgetRepository = budgetRepository;
            _userRepository = userRepository;
            _logger = logger;
            _accountRepository = accountRepository;
        }

        public async Task<BudgetModel> CreateBudgetAsync(BudgetModel model)
        {
            try
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
                budget.Needs = model.Needs;

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
        public string GetCurrentTab()
        {
            return "Needs";
        }

        public void SetTab(string tabName)
        {
            _currentTab = tabName;
        }

        public decimal GetNeedsBudget()
        {
            return 500.00M;
        }

        public async Task<bool> SubmitNeedsAsync(decimal needsAmount, int userId)
        {
            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                return false;
            }

            _needsBudget = needsAmount;
            _budgetRepository.SaveNeedsBudget(needsAmount, userId);
            return true;
        }

        public async Task UpdateNeedsAmount(int budgetId, decimal needsAmount)
        {
            var budget = await _budgetRepository.GetByIdAsync(budgetId);
            if (budget != null)
            {
                budget.Needs = needsAmount;
                await _budgetRepository.UpdateAsync(budget);
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
