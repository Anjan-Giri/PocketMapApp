namespace PocketMapApp.Models
{
    //model for debt
    public class Debt
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Source { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsCleared { get; set; }
        public string Notes { get; set; }
        public User User { get; set; }
    }
}
