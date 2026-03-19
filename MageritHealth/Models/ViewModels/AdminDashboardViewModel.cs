namespace MageritHealth.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalPacientes { get; set; }
        public int TotalDoctoresActivos { get; set; }
        public int CitasProgramadas { get; set; }
        public int AnaliticasPendientes { get; set; }

        public List<Cita> ProximasCitas { get; set; }
    }
}
