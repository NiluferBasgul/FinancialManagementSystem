using FinancialManagementSystem.Core.Data;
using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FinancialManagementSystem.Tests.Repositories
{
    public class AccountRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> GetInMemoryDbContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void GetById_ExistingAccount_ReturnsAccount()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var accountId = 1;

            using (var context = new ApplicationDbContext(options))
            {
                context.Accounts.Add(new Account { Id = accountId, Balance = 1000 });
                context.SaveChanges();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new AccountRepository(context);
                var result = repository.GetById(accountId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(1000, result.Balance);
            }
        }

        [Fact]
        public void GetById_NonExistingAccount_ReturnsNull()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var nonExistingAccountId = 999;

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new AccountRepository(context);
                var result = repository.GetById(nonExistingAccountId);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public void Update_ExistingAccount_UpdatesAccountInDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var accountId = 1;

            using (var context = new ApplicationDbContext(options))
            {
                context.Accounts.Add(new Account { Id = accountId, Balance = 1000 });
                context.SaveChanges();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var repository = new AccountRepository(context);
                var accountToUpdate = repository.GetById(accountId);
                accountToUpdate.Balance = 2000;
                repository.Update(accountToUpdate);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var updatedAccount = context.Accounts.Find(accountId);
                Assert.NotNull(updatedAccount);
                Assert.Equal(2000, updatedAccount.Balance);
            }
        }
    }
}
