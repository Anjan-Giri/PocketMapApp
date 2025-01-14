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

        //method to add transaction, returns sucess or error message in case of failure
        public async Task<(bool success, string error)> AddTransaction(Transaction transaction)
        {
            try
            {
                //if added transaction type is debit
                if (transaction.Type == TransactionType.Debit)
                {
                    var balance = await GetBalance(transaction.UserId); //getting available balance from getbalance method

                    //if balance is insufficient
                    if (balance < transaction.Amount)
                        return (false, "Insufficient balance");
                }

                _context.Transactions.Add(transaction); //adding transaction to database transaction table
                await _context.SaveChangesAsync(); //saving the transaction object in the database
                return (true, null); //success
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        //method to calculate the total available balance of provided userid
        public async Task<decimal> GetBalance(int userId)
        {
            //total credit amount
            var credits = await _context.Transactions
                .Where(t => t.UserId == userId && t.Type == TransactionType.Credit)
                .SumAsync(t => t.Amount);

            //total debit amount
            var debits = await _context.Transactions
                .Where(t => t.UserId == userId && t.Type == TransactionType.Debit)
                .SumAsync(t => t.Amount);

            //available balance
            return credits - debits;
        }
    }
}

//using PocketMapApp.Data;
//using PocketMapApp.Models;
//using Microsoft.EntityFrameworkCore;

//namespace PocketMapApp.Services
//{
//    public class TransactionService
//    {
//        private readonly DatabaseContext _context;

//        public TransactionService(DatabaseContext context)
//        {
//            _context = context;
//        }

//        public async Task<(bool success, string error)> AddTransaction(Transaction transaction)
//        {
//            try
//            {
//                // Basic validation
//                if (transaction == null)
//                    return (false, "Transaction data is invalid");

//                // Check if user exists
//                var user = await _context.Users.FindAsync(transaction.UserId);
//                if (user == null)
//                    return (false, "User not found");

//                // Check balance for debit transactions
//                if (transaction.Type == TransactionType.Debit)
//                {
//                    var balance = await GetBalance(transaction.UserId);
//                    if (balance < transaction.Amount)
//                        return (false, "Insufficient balance");
//                }

//                // Clean up the data
//                transaction.Title = transaction.Title?.Trim();
//                transaction.Notes = transaction.Notes?.Trim();
//                transaction.Tags = transaction.Tags?.Select(t => t.Trim())
//                                             .Where(t => !string.IsNullOrEmpty(t))
//                                             .ToList() ?? new List<string>();

//                _context.Transactions.Add(transaction);

//                try
//                {
//                    await _context.SaveChangesAsync();
//                    return (true, null);
//                }
//                catch (DbUpdateException dbEx)
//                {
//                    return (false, $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
//                }
//            }
//            catch (Exception ex)
//            {
//                return (false, $"An unexpected error occurred: {ex.Message}");
//            }
//        }

//        public async Task<decimal> GetBalance(int userId)
//        {
//            try
//            {
//                var credits = await _context.Transactions
//                    .Where(t => t.UserId == userId && t.Type == TransactionType.Credit)
//                    .SumAsync(t => t.Amount);

//                var debits = await _context.Transactions
//                    .Where(t => t.UserId == userId && t.Type == TransactionType.Debit)
//                    .SumAsync(t => t.Amount);

//                return credits - debits;
//            }
//            catch (Exception)
//            {
//                // If there's an error calculating balance, return 0 to prevent transactions
//                return 0;
//            }
//        }
//    }
//}