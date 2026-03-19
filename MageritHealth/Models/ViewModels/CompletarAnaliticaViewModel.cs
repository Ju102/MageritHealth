using System.ComponentModel.DataAnnotations;

namespace MageritHealth.Models.ViewModels
{
    public class CompletarAnaliticaViewModel
    {
        public int IdAnalitica { get; set; }
        public string NombrePaciente { get; set; }
        public string DniPaciente { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string NombreDoctor { get; set; }
        public string MotivoCita { get; set; }
        public string EstadoActual { get; set; }

        public string NotasDoctor { get; set; }

        [Required(ErrorMessage = "Debe añadir al menos un parámetro.")]
        public List<int> TiposMedicion { get; set; }

        [Required(ErrorMessage = "Debe introducir los valores de las mediciones.")]
        public List<decimal> Valores { get; set; }

        public CompletarAnaliticaViewModel()
        {
            TiposMedicion = new List<int>();
            Valores = new List<decimal>();
        }
    }
}