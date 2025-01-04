namespace PocketMapApp.Models
{
    //model for user
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PreferredCurrency { get; set; }
    }
}
