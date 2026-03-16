using System.ComponentModel.DataAnnotations;

namespace MageritHealth.Models.ViewModels
{
    public class EditPacienteViewModel
    {
        public string Telefono { get; set; }
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        public string? ConfirmPassword { get; set; }

        public string Email { get; set; }
    }
}
