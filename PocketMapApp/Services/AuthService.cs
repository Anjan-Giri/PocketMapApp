using PocketMapApp.Models;
using PocketMapApp.Data; //databasecontext
using Microsoft.EntityFrameworkCore;

namespace PocketMapApp.Services
{
    public class AuthService
    {
        private readonly DatabaseContext _context; //declaring _context for interacting with database

        //allowing class to interact with database
        public AuthService(DatabaseContext context) //through databasecontext
        {
            _context = context;
        }

        //method to register user taskbool returns whether the registration is success or not
        public async Task<bool> RegisterUser(string username, string password, string currency)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Username == username)) //checking if the username already exists
                    return false;

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(password); //using bcrypt to store password for securely, not in plain text

                //creating user object
                var user = new User
                {
                    Username = username,
                    PasswordHash = passwordHash,
                    PreferredCurrency = currency
                };

                _context.Users.Add(user); //user object is added to the database user table
                await _context.SaveChangesAsync(); //saving the user object to database
                return true; //for successfully registered user
            }
            //catching and logging any exceptions occuring in the process
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RegisterUser: {ex}");
                Console.WriteLine($"Error in RegisterUser: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        //method to login user taskuser returns user or null if authentication fails
        public async Task<User> LoginUser(string username, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username); //search for existing user by the provided username
                //if there is no user, login fails
                if (user == null)
                    return null;

                bool validPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash); //comparing enterd password with existing one using bcrypt verify
                return validPassword ? user : null; //if password is matched return the user else login fails
            }
            //logging exceptions
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoginUser: {ex}");
                throw;
            }
        }
    }
}
