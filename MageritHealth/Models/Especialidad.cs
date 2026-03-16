using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MageritHealth.Models
{
    [Table("ESPECIALIDADES")]
    public class Especialidad
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdEspecialidad { get; set; }

        public string NombreEspecialidad { get; set; }

        // RELACIONES: Una especialidad tiene muchos doctores (usuarios)
        public virtual ICollection<Usuario> Doctores { get; set; }
    }
}
