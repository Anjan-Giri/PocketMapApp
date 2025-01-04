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
