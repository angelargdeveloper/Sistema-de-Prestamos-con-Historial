using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlMaximo.Core.Entities
{
    [Table("Empleados")]
    public class Empleado
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string NumNomina { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Apellido1 { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Apellido2 { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        // Propiedad calculada para nombre completo
        [NotMapped]
        public string NombreCompleto => $"{Nombres} {Apellido1}{(!string.IsNullOrEmpty(Apellido2) ? $" {Apellido2}" : "")}";

        // Navegación
        public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
        public virtual ICollection<EmpleadoAutorizador> EmpleadoAutorizadores { get; set; } = new List<EmpleadoAutorizador>();
    }
}