using FinancialManagementSystem.Core.Entities;
using FinancialManagementSystem.Core.Repositories;
using FinancialManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FinancialManagementSystem.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly Mock<ILogger<UserRepository>> _userRepository = new Mock<ILogger<UserRepository>>();
        private DbContextOptions<ApplicationDbContext> GetInMemoryDbContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetByIdAsync_ExistingUser_ReturnsUser()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            using (var context = new ApplicationDbContext(options))
            {
                var user = new User { Id = 1, Username = "testuser", Email = "test@example.com" };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var logger = new Mock<ILogger<UserRepository>>().Object;
                var repository = new UserRepository(context, logger);
                var result = await repository.GetByIdAsync(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal("testuser", result.Username);
            }
        }

        [Fact]
        public async Task GetByUsernameAsync_ExistingUsername_ReturnsUser()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            using (var context = new ApplicationDbContext(options))
            {
                var user = new User { Id = 1, Username = "testuser", Email = "test@example.com" };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var logger = new Mock<ILogger<UserRepository>>().Object;
                var repository = new UserRepository(context, logger);
                var result = await repository.GetByUsernameAsync("testuser");

                // Assert
                Assert.NotNull(result);
                Assert.Equal("test@example.com", result.Email);
            }
        }

        [Fact]
        public async Task AddAsync_NewUser_AddsUserToDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var newUser = new User { Username = "newuser", Email = "new@example.com" };

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var logger = new Mock<ILogger<UserRepository>>().Object;
                var repository = new UserRepository(context, logger);
                await repository.AddAsync(newUser);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var addedUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "newuser");
                Assert.NotNull(addedUser);
                Assert.Equal("new@example.com", addedUser.Email);
            }
        }

        [Fact]
        public async Task UpdateAsync_ExistingUser_UpdatesUserInDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var userId = 1;
            using (var context = new ApplicationDbContext(options))
            {
                var user = new User { Id = userId, Username = "testuser", Email = "test@example.com" };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                    var logger = new Mock<ILogger<UserRepository>>().Object;
                    var repository = new UserRepository(context, logger);
                    var userToUpdate = await repository.GetByIdAsync(userId);
                userToUpdate.Email = "updated@example.com";
                await repository.UpdateAsync(userToUpdate);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var updatedUser = await context.Users.FindAsync(userId);
                Assert.NotNull(updatedUser);
                Assert.Equal("updated@example.com", updatedUser.Email);
            }
        }

        [Fact]
        public async Task DeleteAsync_ExistingUser_RemovesUserFromDatabase()
        {
            // Arrange
            var options = GetInMemoryDbContextOptions();
            var userId = 1;
            using (var context = new ApplicationDbContext(options))
            {
                var user = new User { Id = userId, Username = "testuser", Email = "test@example.com" };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var logger = new Mock<ILogger<UserRepository>>().Object;
                var repository = new UserRepository(context, logger);
                await repository.DeleteAsync(userId);
            }

            // Assert
            using (var context = new ApplicationDbContext(options))
            {
                var deletedUser = await context.Users.FindAsync(userId);
                Assert.Null(deletedUser);
            }
        }
    }
}