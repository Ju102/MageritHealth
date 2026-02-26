namespace MageritHealth.Models
{
    public class PatientDashboard // Próxima cita, lista de medicamentos
    {
        // Próxima cita
        public Appointment NextAppointment { get; set; }

        // Medicamentos activos. TEMPORAL: Medication no corresponde al objeto devuelto por la BBDD
        public List<Medication> ActiveMedicationList { get; set; }

        //
    }
}
