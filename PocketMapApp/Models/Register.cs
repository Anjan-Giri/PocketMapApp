using System.ComponentModel.DataAnnotations;

namespace PocketMapApp.Models
{
    public class registrationModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please select a preferred currency")]
        public string PreferredCurrency { get; set; }
    }
}
