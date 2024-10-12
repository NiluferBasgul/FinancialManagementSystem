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
        public async Task GetIncomeAsync_ExistingIncome_ReturnsIncomeModel()
        {
            // Arrange
            var incomeId = 1;
            var income = new Income { Id = incomeId, UserId = 1, Amount = 1000, Date = DateTime.Now, Description = "Salary", Category = "Work" };
            _mockIncomeRepository.Setup(r => r.GetByIdAsync(incomeId)).ReturnsAsync(income);

            // Act
            var result = await _incomeService.GetIncomeAsync(incomeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(incomeId, result.Id);
            Assert.Equal("Salary", result.Description);
        }

        [Fact]
        public async Task GetIncomeAsync_NonExistingIncome_ReturnsNull()
        {
            // Arrange
            var incomeId = 999;
            _mockIncomeRepository.Setup(r => r.GetByIdAsync(incomeId)).ReturnsAsync((Income)null);

            // Act
            var result = await _incomeService.GetIncomeAsync(incomeId);

            // Assert
            Assert.Null(result);
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

        [Fact]
        public async Task AddIncomeAsync_ThrowsException_LogsError()
        {
            // Arrange
            var incomeModel = new IncomeModel { UserId = 1, Amount = 1000, Date = DateTime.Now, Description = "Salary", Category = "Work" };
            _mockIncomeRepository.Setup(r => r.AddAsync(It.IsAny<Income>())).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _incomeService.AddIncomeAsync(incomeModel));
            Assert.Equal("Database error", ex.Message);
        }

        [Fact]
        public async Task UpdateIncomeAsync_ExistingIncome_ReturnsUpdatedIncome()
        {
            // Arrange
            var incomeId = 1;
            var model = new IncomeModel { Id = incomeId, UserId = 1, Amount = 1500, Date = DateTime.Now, Description = "Updated Salary", Category = "Work" };
            var income = new Income { Id = incomeId, UserId = 1, Amount = 1000, Date = DateTime.Now, Description = "Salary", Category = "Work" };

            _mockIncomeRepository.Setup(r => r.GetByIdAsync(incomeId)).ReturnsAsync(income);
            _mockIncomeRepository.Setup(r => r.UpdateAsync(It.IsAny<Income>())).Returns(Task.CompletedTask);

            // Act
            var result = await _incomeService.UpdateIncomeAsync(model);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1500, result.Amount);
            Assert.Equal("Updated Salary", result.Description);
        }

        [Fact]
        public async Task UpdateIncomeAsync_NonExistingIncome_ReturnsNull()
        {
            // Arrange
            var model = new IncomeModel { Id = 999, UserId = 1, Amount = 1000, Date = DateTime.Now, Description = "Salary", Category = "Work" };
            _mockIncomeRepository.Setup(r => r.GetByIdAsync(model.Id)).ReturnsAsync((Income)null);

            // Act
            var result = await _incomeService.UpdateIncomeAsync(model);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteIncomeAsync_ExistingIncome_DeletesIncome()
        {
            // Arrange
            var incomeId = 1;
            var income = new Income { Id = incomeId, UserId = 1, Amount = 1000, Date = DateTime.Now, Description = "Salary", Category = "Work" };

            _mockIncomeRepository.Setup(r => r.GetByIdAsync(incomeId)).ReturnsAsync(income);
            _mockIncomeRepository.Setup(r => r.DeleteAsync(income)).Returns(Task.CompletedTask);

            // Act
            await _incomeService.DeleteIncomeAsync(incomeId);

            // Assert
            _mockIncomeRepository.Verify(r => r.DeleteAsync(income), Times.Once);
        }

        [Fact]
        public async Task GetTotalIncomeForPeriodAsync_ValidPeriod_ReturnsTotalIncome()
        {
            // Arrange
            var userId = 1;
            var startDate = DateTime.Now.AddMonths(-1);
            var endDate = DateTime.Now;
            var totalIncome = 5000M;

            _mockIncomeRepository.Setup(r => r.GetTotalIncomeForPeriodAsync(userId, startDate, endDate)).ReturnsAsync(totalIncome);

            // Act
            var result = await _incomeService.GetTotalIncomeForPeriodAsync(userId, startDate, endDate);

            // Assert
            Assert.Equal(totalIncome, result);
        }

        [Fact]
        public async Task GetTotalIncomeForPeriodAsync_ThrowsException_LogsError()
        {
            // Arrange
            var userId = 1;
            var startDate = DateTime.Now.AddMonths(-1);
            var endDate = DateTime.Now;
            _mockIncomeRepository.Setup(r => r.GetTotalIncomeForPeriodAsync(userId, startDate, endDate)).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _incomeService.GetTotalIncomeForPeriodAsync(userId, startDate, endDate));
            Assert.Equal("Database error", ex.Message);
        }
    }
}
