namespace MageritHealth.Models.ViewModels
{
    public class DoctorDashboardViewModel
    {
        public int TotalCitasHoy { get; set; }
        public int AnaliticasPendientes { get; set; }
        public int PrescripcionesRecientes { get; set; }

        public List<Cita> AgendaHoy { get; set; }

        public List<Usuario> UltimosPacientes { get; set; }
    }
}