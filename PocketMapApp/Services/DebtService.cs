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

        //method to add debt, returns sucess or error message in case of failure
        public async Task<(bool success, string error)> AddDebt(Debt debt)
        {
            try
            {
                debt.CreatedDate = DateTime.Now;
                debt.IsCleared = false; //debt is unpaid when adding it

                _context.Debts.Add(debt); //debt object is added to the database debt table
                await _context.SaveChangesAsync(); //saving the changes
                return (true, null); //when success
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        //method to mark debt as paid
        public async Task<(bool success, string error)> ClearDebt(int debtId, int userId)
        {
            try
            {
                //finding the matching debt through provided user id and debt id
                var debt = await _context.Debts
                    .FirstOrDefaultAsync(d => d.Id == debtId && d.UserId == userId); //firstordefaultasync returns found debt or null if not found

                if (debt == null)
                    return (false, "Debt not found");

                //if iscleared value is already true, return false and already cleared message
                if (debt.IsCleared)
                    return (false, "Debt is already cleared");

                //adding all credit tranactions amount
                var balanceCredit = await _context.Transactions
                    .Where(t => t.UserId == userId && t.Type == TransactionType.Credit)
                    .SumAsync(t => t.Amount);

                //adding all debit tranactions amount
                var balanceDebit = await _context.Transactions
                    .Where(t => t.UserId == userId && t.Type == TransactionType.Debit)
                    .SumAsync(t => t.Amount);

                //available balance
                var availableBalance = balanceCredit - balanceDebit;

                //if available balance is insufficient
                if (availableBalance < debt.Amount)
                    return (false, "Insufficient balance to clear debt");

                debt.IsCleared = true; //marking debt as cleared
                await _context.SaveChangesAsync(); //saving changes in the database
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
