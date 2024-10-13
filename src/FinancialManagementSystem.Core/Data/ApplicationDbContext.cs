using FinancialManagementSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.Core.Data
{
    [ExcludeFromCodeCoverage]
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "Server=mysql.railway.internal;Port=3306;Database=railway;User=root;Password=dDoOkkMytDinrgTTbfIkOyEQzqjfNOYy",
                new MySqlServerVersion(new Version(9, 0, 0)) // Adjust MySQL version as necessary
            );
        }

        // Remove the OnConfiguring method since the options are now passed via Dependency Injection

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<BudgetCategory> BudgetCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Balance).HasPrecision(18, 2);
            });

            // Budget entity configuration
            modelBuilder.Entity<Budget>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.HasIndex(e => e.UserId);
            });

            modelBuilder.Entity<BudgetCategory>(entity =>
            {
                entity.HasOne(bc => bc.NeedsBudget)
                    .WithMany(b => b.Needs)
                    .HasForeignKey(bc => bc.NeedsBudgetId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_BudgetCategory_NeedsBudget");

                entity.HasOne(bc => bc.WantsBudget)
                    .WithMany(b => b.Wants)
                    .HasForeignKey(bc => bc.WantsBudgetId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_BudgetCategory_WantsBudget");

                entity.HasOne(bc => bc.SavingsBudget)
                    .WithMany(b => b.Savings)
                    .HasForeignKey(bc => bc.SavingsBudgetId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_BudgetCategory_SavingsBudget");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                // Add any other configurations for Transaction entity
            });

            // Income entity configuration
            modelBuilder.Entity<Income>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Date);
            });

            // Reminder entity configuration
            modelBuilder.Entity<Reminder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.DueDate);
            });

            // Goal entity configuration
            modelBuilder.Entity<Goal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.CurrentAmount).HasPrecision(18, 2);
                entity.Property(e => e.TargetAmount).HasPrecision(18, 2);
                entity.HasIndex(e => e.UserId);
            });

            // Expense entity configuration
            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Date);
            });

            // Account entity configuration
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.Balance).HasPrecision(18, 2);
                entity.HasOne(a => a.User)
                      .WithMany(u => u.Accounts)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
