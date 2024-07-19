using FinancialManagementSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancialManagementSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Goal> Goals { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.UserId);

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.Date);

            modelBuilder.Entity<Budget>()
                .HasIndex(b => b.UserId);

            modelBuilder.Entity<Income>()
                .HasIndex(i => i.UserId);

            modelBuilder.Entity<Reminder>()
                .HasIndex(r => r.UserId);

            modelBuilder.Entity<Goal>()
                .HasIndex(g => g.UserId);
        }
    }
}