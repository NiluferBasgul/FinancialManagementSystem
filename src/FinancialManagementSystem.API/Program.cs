using FinancialManagementSystem.API.Middleware;
using FinancialManagementSystem.Core.Interfaces;
using FinancialManagementSystem.Core.Repositories;
using FinancialManagementSystem.Core.Services;
using FinancialManagementSystem.Infrastructure.Data;
using FinancialManagementSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public class Program
{
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();
            builder.Services.AddScoped<IBudgetService, BudgetService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
            builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();

            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
            app.UseMiddleware<AuthMiddleware>();

            app.MapControllers();

            app.Run();
        }
    
}