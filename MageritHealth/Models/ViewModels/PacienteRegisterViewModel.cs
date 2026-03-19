using System.ComponentModel.DataAnnotations;

namespace MageritHealth.Models.ViewModels
{
    public class PacienteRegisterViewModel : UserRegisterViewModel
    {
        public string Rol { get; set; } = "paciente";

        [Required(ErrorMessage = "El número de asegurado es obligatorio.")]
        public string NumeroAsegurado { get; set; }

        // Informacion Clinica

        public string GrupoSanguineo { get; set; }
        public decimal PesoActual { get; set; }
        public string ContactoEmergenciaNombre { get; set; }
        public string ContactoEmergenciaTelefono { get; set; }
    }
}
