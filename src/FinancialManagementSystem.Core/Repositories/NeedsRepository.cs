using FinancialManagementSystem.Core.Data;
using FinancialManagementSystem.Core.Interfaces;

namespace FinancialManagementSystem.Core.Repositories
{
    public class NeedsRepository : INeedsRepository
    {
        private readonly ApplicationDbContext _context;

        public NeedsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void SaveNeedsBudget(decimal needsAmount, int userId)
        {
            var budget = _context.Budgets.FirstOrDefault(b => b.UserId == userId);
            if (budget != null)
            {
                budget.Needs = needsAmount;
                _context.SaveChanges();
            }
        }
    }
}

