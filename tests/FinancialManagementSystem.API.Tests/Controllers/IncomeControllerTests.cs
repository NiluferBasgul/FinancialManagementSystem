using FinancialManagementSystem.API.Controllers;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace FinancialManagementSystem.Tests.Controllers
{
    public class IncomeControllerTests
    {
        private readonly Mock<IIncomeService> _mockIncomeService;
        private readonly IncomeController _controller;

        public IncomeControllerTests()
        {
            _mockIncomeService = new Mock<IIncomeService>();
            _controller = new IncomeController(_mockIncomeService.Object);
            SetupControllerContext();
        }

        private void SetupControllerContext()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetIncomes_ReturnsOkResultWithIncomes()
        {
            // Arrange
            var userId = 1;
            var incomes = new List<IncomeModel>
            {
                new IncomeModel { Id = 1, UserId = userId, Amount = 1000, Date = DateTime.Now, Description = "Salary", Category = "Work" },
                new IncomeModel { Id = 2, UserId = userId, Amount = 500, Date = DateTime.Now, Description = "Freelance", Category = "Work" }
            };
            _mockIncomeService.Setup(s => s.GetIncomesAsync(userId)).ReturnsAsync(incomes);

            // Act
            var result = await _controller.GetIncomes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<IncomeModel>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task AddIncome_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var incomeModel = new IncomeModel { UserId = 1, Amount = 1000, Date = DateTime.Now, Description = "Salary", Category = "Work" };
            _mockIncomeService.Setup(s => s.AddIncomeAsync(It.IsAny<IncomeModel>())).ReturnsAsync(incomeModel);

            // Act
            var result = await _controller.AddIncome(incomeModel);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetIncome", createdAtActionResult.ActionName);
            var returnValue = Assert.IsType<IncomeModel>(createdAtActionResult.Value);
            Assert.Equal(incomeModel.Amount, returnValue.Amount);
        }

        // Add more tests for other actions
    }
}