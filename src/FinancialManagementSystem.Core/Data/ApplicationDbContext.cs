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

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseMySql(
        //        "Server=junction.proxy.rlwy.net;Port=47703;Database=railway;User=root;Password=dDoOkkMytDinrgTTbfIkOyEQzqjfNOYy",
        //        new MySqlServerVersion(new Version(9, 0, 0)) // Adjust MySQL version as necessary
        //    );
        //}

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

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Balance).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Budget>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.HasIndex(e => e.UserId);

                entity.HasMany(b => b.Needs)
                      .WithOne(bc => bc.NeedsBudget)
                      .HasForeignKey(bc => bc.NeedsBudgetId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired(false);

                entity.HasMany(b => b.Wants)
                      .WithOne(bc => bc.WantsBudget)
                      .HasForeignKey(bc => bc.WantsBudgetId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired(false);

                entity.HasMany(b => b.Savings)
                      .WithOne(bc => bc.SavingsBudget)
                      .HasForeignKey(bc => bc.SavingsBudgetId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired(false); 
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.Amount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Income>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Date);
            });

            modelBuilder.Entity<Reminder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.DueDate);
            });

            modelBuilder.Entity<Goal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.CurrentAmount).HasPrecision(18, 2);
                entity.Property(e => e.TargetAmount).HasPrecision(18, 2);
                entity.HasIndex(e => e.UserId);
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Date);
            });

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
