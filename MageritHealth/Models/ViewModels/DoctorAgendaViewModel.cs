using System;
using System.Collections.Generic;
using System.Globalization; // <-- Necesario para el InvariantCulture
using MageritHealth.Models;

namespace MageritHealth.Models.ViewModels
{
    public class DoctorAgendaViewModel
    {
        public DateTime FechaDada { get; set; }
        public List<Cita> Citas { get; set; }

        public int TotalCitas { get; set; }
        public int CitasCompletadas { get; set; }
        public int CitasEnCurso { get; set; }
        public int CitasPendientes { get; set; }
        public int CitasCanceladas { get; set; }

        // ESTA ES LA CORRECCIÓN: Usamos ToString("0.##", CultureInfo.InvariantCulture)
        public string PorcentajeCompletadas => TotalCitas > 0
            ? (CitasCompletadas * 100.0 / TotalCitas).ToString("0.##", CultureInfo.InvariantCulture) + "%"
            : "0%";

        public string PorcentajeEnCurso => TotalCitas > 0
            ? (CitasEnCurso * 100.0 / TotalCitas).ToString("0.##", CultureInfo.InvariantCulture) + "%"
            : "0%";

        public string PorcentajePendientes => TotalCitas > 0
            ? (CitasPendientes * 100.0 / TotalCitas).ToString("0.##", CultureInfo.InvariantCulture) + "%"
            : "0%";

        public string PorcentajeCanceladas => TotalCitas > 0
            ? (CitasCanceladas * 100.0 / TotalCitas).ToString("0.##", CultureInfo.InvariantCulture) + "%"
            : "0%";
    }
}