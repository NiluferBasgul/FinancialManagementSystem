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
    public class TransferControllerTests
    {
        private readonly Mock<IBudgetService> _mockBudgetService;
        private readonly TransferController _controller;

        public TransferControllerTests()
        {
            _mockBudgetService = new Mock<IBudgetService>();
            _controller = new TransferController(_mockBudgetService.Object);

            // Setup ClaimsPrincipal for authorized user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public void TransferFunds_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var transferRequest = new TransferRequest
            {
                FromAccountId = 1,
                ToAccountId = 2,
                Amount = 100
            };

            // Act
            var result = _controller.TransferFunds(transferRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void TransferFunds_SameAccount_ReturnsBadRequest()
        {
            // Arrange
            var transferRequest = new TransferRequest
            {
                FromAccountId = 1,
                ToAccountId = 1,
                Amount = 100
            };

            // Act
            var result = _controller.TransferFunds(transferRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cannot transfer to the same account.", badRequestResult.Value);
        }

        [Fact]
        public void TransferFunds_InsufficientFunds_ThrowsException_ReturnsBadRequest()
        {
            // Arrange
            var transferRequest = new TransferRequest
            {
                FromAccountId = 1,
                ToAccountId = 2,
                Amount = 100
            };
            _mockBudgetService.Setup(s => s.TransferFunds(transferRequest))
                .Throws(new InvalidOperationException("Insufficient funds"));

            // Act
            var result = _controller.TransferFunds(transferRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
