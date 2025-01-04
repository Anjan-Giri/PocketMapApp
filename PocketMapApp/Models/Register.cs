using System.ComponentModel.DataAnnotations;

namespace PocketMapApp.Models
{
    //model for registration
    public class RegisterModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please select a preferred currency")]
        public string PreferredCurrency { get; set; }
    }
}
