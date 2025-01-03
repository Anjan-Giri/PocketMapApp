using PocketMapApp.Models;
using PocketMapApp.Data;
using Microsoft.EntityFrameworkCore;

namespace PocketMapApp.Services
{
    public class AuthService
    {
        private readonly DatabaseContext _context;

        public AuthService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterUser(string username, string password, string currency)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Username == username))
                    return false;

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                var user = new User
                {
                    Username = username,
                    PasswordHash = passwordHash,
                    PreferredCurrency = currency
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RegisterUser: {ex}");
                Console.WriteLine($"Error in RegisterUser: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<User> LoginUser(string username, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                    return null;

                bool validPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                return validPassword ? user : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoginUser: {ex}");
                throw;
            }
        }
    }
}
