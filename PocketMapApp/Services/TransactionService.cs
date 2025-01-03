using PocketMapApp.Data;
using PocketMapApp.Models;
using Microsoft.EntityFrameworkCore;

namespace PocketMapApp.Services
{
    public class TransactionService
    {
        private readonly DatabaseContext _context;

        public TransactionService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<(bool success, string error)> AddTransaction(Transaction transaction)
        {
            try
            {
                if (transaction.Type == TransactionType.Debit)
                {
                    var balance = await GetBalance(transaction.UserId);
                    if (balance < transaction.Amount)
                        return (false, "Insufficient balance");
                }

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<decimal> GetBalance(int userId)
        {
            var credits = await _context.Transactions
                .Where(t => t.UserId == userId && t.Type == TransactionType.Credit)
                .SumAsync(t => t.Amount);

            var debits = await _context.Transactions
                .Where(t => t.UserId == userId && t.Type == TransactionType.Debit)
                .SumAsync(t => t.Amount);

            return credits - debits;
        }
    }
}
