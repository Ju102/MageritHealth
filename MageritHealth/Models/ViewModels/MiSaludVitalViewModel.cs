using MageritHealth.Models;

namespace MageritHealth.ViewModels
{
    public class MiSaludVitalViewModel
    {
        // Lista con los últimos valores de las constantes del paciente
        public List<MedicionResumen> Mediciones { get; set; } = new List<MedicionResumen>();

        // Lista de analíticas (históricas y futuras)
        public List<Analitica> Analiticas { get; set; } = new List<Analitica>();

        public List<AntecedenteMedico> AntecedentesMedicos { get; set; } = new List<AntecedenteMedico>();
    }

    public class MedicionResumen
    {
        public string Nombre { get; set; }
        public string Unidad { get; set; }
        public decimal Valor { get; set; }
        public decimal Minimo { get; set; }
        public decimal Maximo { get; set; }
        public DateTime Fecha { get; set; }

        // --- Lógica Auxiliar para la Vista ---

        // Calcula si está alto, bajo o normal
        public string EstadoRango => Valor > Maximo ? "Alto" : (Valor < Minimo ? "Bajo" : "En Rango");

        // Asigna el color de Bootstrap según el estado
        public string ColorClass => EstadoRango == "En Rango" ? "success" : (EstadoRango == "Alto" ? "danger" : "warning");

        // Calcula el % para la progress bar (evitando dividir por cero o desbordar)
        public int PorcentajeBarra
        {
            get
            {
                if (Maximo == Minimo) return 50; // Seguridad
                decimal rangoTotal = Maximo - Minimo;

                // Calculamos cuánto representa el valor dentro del rango min-max
                // Asumimos que el "0%" visual es el mínimo y el "100%" visual es el máximo (o más)
                decimal porcentaje = ((Valor - Minimo) / rangoTotal) * 100;

                if (porcentaje < 5) return 5; // Mínimo visual
                if (porcentaje > 100) return 100; // Máximo visual

                return (int)porcentaje;
            }
        }
    }
}