using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    [Table("USUARIOS")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido1 { get; set; }
        public string? Apellido2 { get; set; }
        public string Dni { get; set; }
        public DateOnly FechaNacimiento { get; set; }
        public string Telefono { get; set; }
        public string Genero { get; set; }
        public string Direccion { get; set; }

        public string Email { get; set; }
        public string Pass { get; set; }
        public string Rol { get; set; }

        public int? IdEspecialidad { get; set; }
        public string? NumeroColegiado { get; set; }

        public string? NumeroAsegurado { get; set; }

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // RELACIONES (FKs entrantes)
        [ForeignKey("IdEspecialidad")]
        public virtual Especialidad? Especialidad { get; set; }

        // RELACIONES (Colecciones que dependen de este usuario)
        public virtual InfoClinicaPaciente InfoClinica { get; set; }
        public virtual ICollection<AntecedenteMedico> Antecedentes { get; set; }
        public virtual Credencial Credencial { get; set; }

        // Al traer la lista de citas, se pueden diferenciar las que son como paciente o como doctor gracias a las propiedades inversas
        [InverseProperty("Paciente")]
        public virtual ICollection<Cita> CitasComoPaciente { get; set; }
        [InverseProperty("Doctor")]
        public virtual ICollection<Cita> CitasComoDoctor { get; set; }
        
        [InverseProperty("Paciente")]
        public virtual ICollection<DoctorPaciente> MedicosAsignados { get; set; } // Solo para pacientes, lista de doctores asignados
        [InverseProperty("Doctor")]
        public virtual ICollection<DoctorPaciente> PacientesAsignados { get; set; } // Solo para doctores, lista de pacientes asignados

    }
}
