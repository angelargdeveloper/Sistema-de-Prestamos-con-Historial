using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlMaximo.Core.Entities
{
    [Table("PrestamosHistorial")]
    public class PrestamoHistorial
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PrestamoId { get; set; }

        [Required]
        public int EmpleadoId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CantidadTotalPrestada { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CantidadTotalAPagar { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal InteresAprobado { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal InteresMoratorio { get; set; }

        [Required]
        public int TipoPagoAbonoId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? FechaPrimerPago { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAbonadoCapital { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAbonadoIntereses { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Saldo { get; set; }

        [Column(TypeName = "date")]
        public DateTime? FechaFinalPago { get; set; }

        [Required]
        public int AutorPersonaQueAutorizaId { get; set; }

        public string? Notas { get; set; }

        public bool Activo { get; set; }

        [Required]
        [StringLength(20)]
        public string TipoOperacion { get; set; } = string.Empty; // INSERT, UPDATE, DELETE

        public DateTime FechaOperacion { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string? UsuarioOperacion { get; set; }

        // Navegación
        [ForeignKey("PrestamoId")]
        public virtual Prestamo Prestamo { get; set; } = null!;

        [ForeignKey("EmpleadoId")]
        public virtual Empleado Empleado { get; set; } = null!;

        [ForeignKey("TipoPagoAbonoId")]
        public virtual TipoPagoAbono TipoPagoAbono { get; set; } = null!;

        [ForeignKey("AutorPersonaQueAutorizaId")]
        public virtual EmpleadoAutorizador AutorPersonaQueAutoriza { get; set; } = null!;
    }
}