using System.ComponentModel.DataAnnotations;

namespace MageritHealth.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Debes introducir tu email.")]
        public string LoginEmail { get; set; }

        [Required(ErrorMessage = "Debes introducir tu password.")]
        public string LoginPassword { get; set; }
    }
}
