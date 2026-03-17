using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlMaximo.Core.Entities
{
    [Table("TipoPagosAbonos")]
    public class TipoPagoAbono
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string NombreCorto { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Descripcion { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación
        public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
    }
}