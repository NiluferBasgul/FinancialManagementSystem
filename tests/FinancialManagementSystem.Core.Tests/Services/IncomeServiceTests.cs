using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using FinancialManagementSystem.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FinancialManagementSystem.Tests.Services
{
    public class IncomeServiceTests
    {
        private readonly Mock<ILogger<IncomeService>> _mockLogger;
        private readonly Mock<IIncomeRepository> _mockIncomeRepository;
        private readonly IncomeService _incomeService;

        public IncomeServiceTests()
        {
            _mockLogger = new Mock<ILogger<IncomeService>>();
            _mockIncomeRepository = new Mock<IIncomeRepository>();
            _incomeService = new IncomeService(_mockIncomeRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetIncomesAsync_ReturnsIncomeModels()
        {
            // Arrange
            var userId = 1;
            var incomes = new List<Income>
            {
                new Income { Id = 1, UserId = userId, Amount = 1000, Date = DateTime.Now, Description = "Salary", Category = "Work" },
                new Income { Id = 2, UserId = userId, Amount = 500, Date = DateTime.Now, Description = "Freelance", Category = "Work" }
            };
            _mockIncomeRepository.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(incomes);

            // Act
            var result = await _incomeService.GetIncomesAsync(userId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, i => Assert.Equal(userId, i.UserId));
        }

        [Fact]
        public async Task AddIncomeAsync_ReturnsAddedIncomeModel()
        {
            // Arrange
            var incomeModel = new IncomeModel { UserId = 1, Amount = 1000, Date = DateTime.Now, Description = "Salary", Category = "Work" };
            var income = new Income { Id = 1, UserId = 1, Amount = 1000, Date = DateTime.Now, Description = "Salary", Category = "Work" };
            _mockIncomeRepository.Setup(r => r.AddAsync(It.IsAny<Income>())).ReturnsAsync(income);

            // Act
            var result = await _incomeService.AddIncomeAsync(incomeModel);

            // Assert
            Assert.Equal(1, result.Id);
            Assert.Equal(incomeModel.Amount, result.Amount);
            Assert.Equal(incomeModel.Description, result.Description);
        }
    }
}