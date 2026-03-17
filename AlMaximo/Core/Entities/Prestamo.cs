using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlMaximo.Core.Entities
{
    [Table("Prestamos")]
    public class Prestamo
    {
        [Key]
        public int IdPrestamo { get; set; }

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
        public decimal TotalAbonadoCapital { get; set; } = 0.00m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAbonadoIntereses { get; set; } = 0.00m;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Saldo { get; set; }

        [Column(TypeName = "date")]
        public DateTime? FechaFinalPago { get; set; }

        [Required]
        public int AutorPersonaQueAutorizaId { get; set; }

        public string? Notas { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime FechaModificacion { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string? UsuarioCreacion { get; set; }

        [StringLength(100)]
        public string? UsuarioModificacion { get; set; }

        // Navegación
        [ForeignKey("EmpleadoId")]
        public virtual Empleado Empleado { get; set; } = null!;

        [ForeignKey("TipoPagoAbonoId")]
        public virtual TipoPagoAbono TipoPagoAbono { get; set; } = null!;

        [ForeignKey("AutorPersonaQueAutorizaId")]
        public virtual EmpleadoAutorizador AutorPersonaQueAutoriza { get; set; } = null!;

        public virtual ICollection<PrestamoHistorial> HistorialCambios { get; set; } = new List<PrestamoHistorial>();

        // Propiedades calculadas
        [NotMapped]
        public decimal PorcentajePagado => CantidadTotalAPagar > 0 ? ((TotalAbonadoCapital + TotalAbonadoIntereses) / CantidadTotalAPagar) * 100 : 0;

        [NotMapped]
        public bool EstaPagado => Saldo <= 0;

        [NotMapped]
        public string EstadoPago => EstaPagado ? "Pagado" : Saldo < CantidadTotalAPagar ? "En progreso" : "Pendiente";
    }
}