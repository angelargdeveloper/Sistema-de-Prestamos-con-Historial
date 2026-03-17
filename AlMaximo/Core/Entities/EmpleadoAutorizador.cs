using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlMaximo.Core.Entities
{
    [Table("EmpleadosAutorizadores")]
    public class EmpleadoAutorizador
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmpleadoId { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación
        [ForeignKey("EmpleadoId")]
        public virtual Empleado Empleado { get; set; } = null!;

        public virtual ICollection<Prestamo> PrestamosAutorizados { get; set; } = new List<Prestamo>();
    }
}