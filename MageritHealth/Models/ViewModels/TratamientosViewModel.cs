namespace MageritHealth.Models.ViewModels
{
    public class TratamientosViewModel
    {
        // Para las tarjetas verdes "En curso"
        public List<Prescripcion> MedicacionActiva { get; set; }

        // Para la tabla de "Historial de Medicación"
        public List<Prescripcion> HistorialMedicacion { get; set; }

        // Para la tarjeta roja de "Alergias Conocidas"
        public List<AntecedenteMedico> Alergias { get; set; }
    }
}