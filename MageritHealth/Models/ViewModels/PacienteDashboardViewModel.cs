namespace MageritHealth.Models.ViewModels
{
    public class PacienteDashboardViewModel
    {
        public string Nombre { get; set; }
        public string NumeroAsegurado { get; set; }

        public Cita ProximaCita { get; set; }

        public List<Cita> UltimasTresCitas { get; set; }

        public PerfilVital Perfil { get; set; }

        public List<Prescripcion> MedicacionActiva { get; set; }
    }

    public class PerfilVital
    {
        public string TipoSangre { get; set; }
        public decimal Peso { get; set; }

        public int Altura { get; set; }

        public string NombreContacto { get; set; }

        public string TelefonoContacto { get; set; }
    }
}
