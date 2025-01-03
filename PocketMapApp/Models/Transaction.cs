using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PocketMapApp.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        [NotMapped] //for not mapping directly to a column
        public List<string> Tags { get; set; } = new();

        [ForeignKey("UserId")]
        public User User { get; set; }
    }

    public enum TransactionType
    {
        Credit,
        Debit
    }
}
