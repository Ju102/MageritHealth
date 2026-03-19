using System.ComponentModel.DataAnnotations;

namespace MageritHealth.Models.ViewModels
{
    public class ConsultaViewModel
    {
        public int IdCita { get; set; }
        public string MotivoCita { get; set; }
        public string NotasDoctor { get; set; }

        public int IdPaciente { get; set; }
        public string NombrePaciente { get; set; }
        public string ApellidoPaciente { get; set; }
        public string DniPaciente { get; set; }
        public string TelefonoPaciente { get; set; }
        public string NumeroAsegurado { get; set; }
        public string Genero { get; set; }
        public int Edad { get; set; }

        public List<AntecedenteMedico> Antecedentes { get; set; }
        public List<Prescripcion> RecetasActivas { get; set; }
        public List<Analitica> AnaliticasSolicitadasHoy { get; set; }
        public List<Cita> HistorialCitasCompletadas { get; set; }

        public int NuevoMedicamentoId { get; set; }
        public string InstruccionesReceta { get; set; }
        public DateOnly FechaFinReceta { get; set; }

        public string TipoAntecedente { get; set; }
        public string NombreAntecedente { get; set; }
        public string SeveridadAntecedente { get; set; }
        public DateTime FechaDiagnostico { get; set; }
        public string NotasAntecedente { get; set; }

        public DateTime FechaAnalitica { get; set; }
        public string InstruccionesAnalitica { get; set; }

        public ConsultaViewModel()
        {
            Antecedentes = new List<AntecedenteMedico>();
            RecetasActivas = new List<Prescripcion>();
            AnaliticasSolicitadasHoy = new List<Analitica>();
            HistorialCitasCompletadas = new List<Cita>();
        }
    }
}