using Microsoft.Extensions.Logging;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using FinancialManagementSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace FinancialManagementSystem.Core.Services
{
    public class IncomeService : IIncomeService
    {
        private readonly IIncomeRepository _incomeRepository;
        private readonly ILogger<IncomeService> _logger;

        public IncomeService(IIncomeRepository incomeRepository, ILogger<IncomeService> logger)
        {
            _incomeRepository = incomeRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<IncomeModel>> GetIncomesAsync(int userId)
        {
            try
            {
                var incomes = await _incomeRepository.GetByUserIdAsync(userId);
                return incomes.Select(MapToIncomeModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting incomes for user {userId}");
                throw;
            }
        }

        public async Task<IncomeModel> GetIncomeAsync(int id)
        {
            try
            {
                var income = await _incomeRepository.GetByIdAsync(id);
                return income != null ? MapToIncomeModel(income) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting income with id {id}");
                throw;
            }
        }

        public async Task<IncomeModel> AddIncomeAsync(IncomeModel model)
        {
            try
            {
                var income = MapToIncome(model);
                var result = await _incomeRepository.AddAsync(income);
                _logger.LogInformation($"Income added for user {model.UserId}: {model.Amount:C2}");
                return MapToIncomeModel(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding income for user {model.UserId}");
                throw;
            }
        }

        public async Task<IncomeModel> UpdateIncomeAsync(IncomeModel model)
        {
            try
            {
                var income = await _incomeRepository.GetByIdAsync(model.Id);
                if (income == null)
                {
                    _logger.LogWarning($"Income with id {model.Id} not found for update");
                    return null;
                }

                income.Amount = model.Amount;
                income.Date = model.Date;
                income.Description = model.Description;
                income.Category = model.Category;

                await _incomeRepository.UpdateAsync(income);
                _logger.LogInformation($"Income updated: ID {model.Id}, User {model.UserId}");
                return MapToIncomeModel(income);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating income with id {model.Id}");
                throw;
            }
        }

        public async Task DeleteIncomeAsync(int id)
        {
            try
            {
                var income = await _incomeRepository.GetByIdAsync(id);
                if (income != null)
                {
                    await _incomeRepository.DeleteAsync(income);
                    _logger.LogInformation($"Income deleted: ID {id}, User {income.UserId}");
                }
                else
                {
                    _logger.LogWarning($"Attempted to delete non-existent income with id {id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting income with id {id}");
                throw;
            }
        }

        public async Task<decimal> GetTotalIncomeForPeriodAsync(int userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var total = await _incomeRepository.GetTotalIncomeForPeriodAsync(userId, startDate, endDate);
                _logger.LogInformation($"Total income retrieved for user {userId} from {startDate} to {endDate}: {total:C2}");
                return total;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting total income for user {userId} from {startDate} to {endDate}");
                throw;
            }
        }

        private IncomeModel MapToIncomeModel(Income income)
        {
            return new IncomeModel
            {
                Id = income.Id,
                UserId = income.UserId,
                Amount = income.Amount,
                Date = income.Date,
                Description = income.Description,
                Category = income.Category
            };
        }

        private Income MapToIncome(IncomeModel model)
        {
            return new Income
            {
                Id = model.Id,
                UserId = model.UserId,
                Amount = model.Amount,
                Date = model.Date,
                Description = model.Description,
                Category = model.Category
            };
        }
    }
}