using PocketMapApp.Data;
using PocketMapApp.Models;
using Microsoft.EntityFrameworkCore;

namespace PocketMapApp.Services
{
    public class DebtService
    {
        private readonly DatabaseContext _context;

        public DebtService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<(bool success, string error)> AddDebt(Debt debt)
        {
            try
            {
                debt.CreatedDate = DateTime.Now;
                debt.IsCleared = false;
                _context.Debts.Add(debt);
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool success, string error)> ClearDebt(int debtId, int userId)
        {
            try
            {
                var debt = await _context.Debts
                    .FirstOrDefaultAsync(d => d.Id == debtId && d.UserId == userId);

                if (debt == null)
                    return (false, "Debt not found");

                if (debt.IsCleared)
                    return (false, "Debt is already cleared");

                var balance = await _context.Transactions
                    .Where(t => t.UserId == userId && t.Type == TransactionType.Credit)
                    .SumAsync(t => t.Amount);

                if (balance < debt.Amount)
                    return (false, "Insufficient credit balance to clear debt");

                debt.IsCleared = true;
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
