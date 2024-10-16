﻿using FinancialManagementSystem.API.Controllers;
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
                new IncomeModel { Id = 1, UserId = userId, Amount = 1000, Date = DateTime.Now, Tax = "Salary", Type = "Work" },
                new IncomeModel { Id = 2, UserId = userId, Amount = 500, Date = DateTime.Now, Tax = "Freelance", Type = "Work" }
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
            var incomeModel = new IncomeModel { UserId = 1, Amount = 1000, Date = DateTime.Now, Tax = "Salary", Type = "Work" };
            _mockIncomeService.Setup(s => s.AddIncomeAsync(It.IsAny<IncomeModel>())).ReturnsAsync(incomeModel);

            // Act
            var result = await _controller.AddIncome(incomeModel);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetIncome", createdAtActionResult.ActionName);
            var returnValue = Assert.IsType<IncomeModel>(createdAtActionResult.Value);
            Assert.Equal(incomeModel.Amount, returnValue.Amount);
        }

        [Fact]
        public async Task UpdateIncome_ValidIncome_ReturnsNoContentResult()
        {
            // Arrange
            var incomeModel = new IncomeModel { Id = 1, UserId = 1, Amount = 1000, Date = DateTime.Now, Tax = "Salary", Type = "Work" };
            _mockIncomeService.Setup(s => s.UpdateIncomeAsync(incomeModel)).ReturnsAsync(incomeModel);

            // Act
            var result = await _controller.UpdateIncome(incomeModel.Id, incomeModel);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteIncome_ValidIncome_ReturnsNoContentResult()
        {
            // Arrange
            var incomeId = 1;
            _mockIncomeService.Setup(s => s.DeleteIncomeAsync(incomeId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteIncome(incomeId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetTotalIncomeForPeriod_ValidPeriod_ReturnsOkResult()
        {
            // Arrange
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now;
            var totalIncome = 1500m;
            _mockIncomeService.Setup(s => s.GetTotalIncomeForPeriodAsync(It.IsAny<int>(), startDate, endDate)).ReturnsAsync(totalIncome);

            // Act
            var result = await _controller.GetTotalIncomeForPeriod(startDate, endDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }
    }
}
