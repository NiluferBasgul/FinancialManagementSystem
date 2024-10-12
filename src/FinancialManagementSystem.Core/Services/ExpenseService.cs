using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using FinancialManagementSystem.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialManagementSystem.Core.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IIncomeRepository _incomeRepository;
        private readonly ILogger<ExpenseService> _logger;
        public ExpenseService(IExpenseRepository expenseRepository, IIncomeRepository incomeRepository, ILogger<ExpenseService> logger)
        {
            _expenseRepository = expenseRepository;
            _incomeRepository = incomeRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Expense>> GetExpensesByUserAsync(int userId, DateTime from, DateTime to)
        {
            return await _expenseRepository.GetExpensesByUserAsync(userId, from, to);
        }

        public async Task<Expense> AddExpenseAsync(ExpenseModel model, int userId)
        {
            try
            {
                var expense = new Expense
                {
                    UserId = userId,
                    Amount = model.Amount,
                    Description = model.Description,
                    Date = model.Date,
                    Category = model.Category
                };

                return await _expenseRepository.AddExpenseAsync(expense);
            }
            catch (Exception ex)
            {
                // Log the error and rethrow
                _logger.LogError(ex, $"Error adding expense for user {userId}");
                throw;
            }
        }


        public async Task UpdateExpenseAsync(ExpenseModel model)
        {
            var expense = await _expenseRepository.GetExpenseByIdAsync(model.Id);
            if (expense != null)
            {
                expense.Amount = model.Amount;
                expense.Description = model.Description;
                expense.Date = model.Date;
                expense.Category = model.Category;

                await _expenseRepository.UpdateExpenseAsync(expense);
            }
        }

        public async Task DeleteExpenseAsync(int id)
        {
            await _expenseRepository.DeleteExpenseAsync(id);
        }

        public FinancialSummary GetFinancialSummary(int userId)
        {
            var totalIncome = _incomeRepository.GetTotalIncomeByUserId(userId); // Fetch from income repository
            var totalExpenses = _expenseRepository.GetTotalExpensesByUserId(userId); // Fetch from expenses repository
            var totalSavings = totalIncome - totalExpenses;

            return new FinancialSummary
            {
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                TotalSavings = totalSavings
            };
        }
    }

}
