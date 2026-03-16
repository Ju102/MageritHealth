using System.ComponentModel.DataAnnotations;

namespace MageritHealth.Models.ViewModels
{
    public class SolicitarCitaViewModel
    {
        [Required(ErrorMessage = "Por favor, selecciona una especialidad.")]
        [Display(Name = "Especialidad Médica")]
        public int IdEspecialidad { get; set; }

        [Required(ErrorMessage = "Por favor, selecciona un doctor.")]
        [Display(Name = "Doctor / Facultativo")]
        public int IdDoctor { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de la Cita")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "La hora es obligatoria.")]
        [Display(Name = "Franja Horaria")]
        public string Hora { get; set; } // Guardaremos algo como "09:30" o "10:00"

        [Required(ErrorMessage = "El motivo de la consulta es obligatorio.")]
        [StringLength(1000, ErrorMessage = "El motivo no puede superar los 1000 caracteres.")]
        [Display(Name = "Motivo de la Consulta")]
        public string Motivo { get; set; }
    }
}