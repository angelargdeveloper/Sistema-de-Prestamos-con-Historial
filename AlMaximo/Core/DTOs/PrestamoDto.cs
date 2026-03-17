using System.ComponentModel.DataAnnotations;

namespace AlMaximo.Core.DTOs
{
    public class PrestamoDto
    {
        public int IdPrestamo { get; set; }
        public int EmpleadoId { get; set; }
        public string NombreEmpleado { get; set; } = string.Empty;
        public string NumNomina { get; set; } = string.Empty;
        public decimal CantidadTotalPrestada { get; set; }
        public decimal CantidadTotalAPagar { get; set; }
        public decimal InteresAprobado { get; set; }
        public decimal InteresMoratorio { get; set; }
        public int TipoPagoAbonoId { get; set; }
        public string TipoPago { get; set; } = string.Empty;
        public string DescripcionTipoPago { get; set; } = string.Empty;
        public DateTime? FechaPrimerPago { get; set; }
        public decimal TotalAbonadoCapital { get; set; }
        public decimal TotalAbonadoIntereses { get; set; }
        public decimal Saldo { get; set; }
        public DateTime? FechaFinalPago { get; set; }
        public int AutorPersonaQueAutorizaId { get; set; }
        public string NombreAutorizador { get; set; } = string.Empty;
        public string? Notas { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string? UsuarioCreacion { get; set; }
        public string? UsuarioModificacion { get; set; }

        // Propiedades calculadas
        public decimal PorcentajePagado => CantidadTotalAPagar > 0 ? 
            Math.Round(((TotalAbonadoCapital + TotalAbonadoIntereses) / CantidadTotalAPagar) * 100, 2) : 0;
        
        public bool EstaPagado => Saldo <= 0;
        
        public string EstadoPago => EstaPagado ? "Pagado" : 
            Saldo < CantidadTotalAPagar ? "En progreso" : "Pendiente";
        
        public string FechaPrimerPagoFormateada => FechaPrimerPago?.ToString("dd/MMM/yyyy") ?? "N/A";
        public string FechaFinalPagoFormateada => FechaFinalPago?.ToString("dd/MMM/yyyy") ?? "N/A";
    }

    public class CreatePrestamoDto
    {
        [Required(ErrorMessage = "El empleado es requerido")]
        public int EmpleadoId { get; set; }

        [Required(ErrorMessage = "La cantidad total prestada es requerida")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
        public decimal CantidadTotalPrestada { get; set; }

        [Required(ErrorMessage = "La cantidad total a pagar es requerida")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad a pagar debe ser mayor a cero")]
        public decimal CantidadTotalAPagar { get; set; }

        [Required(ErrorMessage = "El interés aprobado es requerido")]
        [Range(0, 100, ErrorMessage = "El interés debe estar entre 0 y 100")]
        public decimal InteresAprobado { get; set; }

        [Required(ErrorMessage = "El interés moratorio es requerido")]
        [Range(0, 100, ErrorMessage = "El interés moratorio debe estar entre 0 y 100")]
        public decimal InteresMoratorio { get; set; }

        [Required(ErrorMessage = "El tipo de pago es requerido")]
        public int TipoPagoAbonoId { get; set; }

        public DateTime? FechaPrimerPago { get; set; }

        public DateTime? FechaFinalPago { get; set; }

        [Required(ErrorMessage = "El autorizador es requerido")]
        public int AutorPersonaQueAutorizaId { get; set; }

        public string? Notas { get; set; }

        [Required(ErrorMessage = "El usuario de creación es requerido")]
        public string UsuarioCreacion { get; set; } = string.Empty;
    }

    public class PrestamoHistorialDto
    {
        public int Id { get; set; }
        public int PrestamoId { get; set; }
        public string NombreEmpleado { get; set; } = string.Empty;
        public decimal CantidadTotalPrestada { get; set; }
        public decimal CantidadTotalAPagar { get; set; }
        public decimal InteresAprobado { get; set; }
        public decimal InteresMoratorio { get; set; }
        public string TipoPago { get; set; } = string.Empty;
        public DateTime? FechaPrimerPago { get; set; }
        public decimal TotalAbonadoCapital { get; set; }
        public decimal TotalAbonadoIntereses { get; set; }
        public decimal Saldo { get; set; }
        public DateTime? FechaFinalPago { get; set; }
        public string NombreAutorizador { get; set; } = string.Empty;
        public string? Notas { get; set; }
        public bool Activo { get; set; }
        public string TipoOperacion { get; set; } = string.Empty; // CREATE, UPDATE, DELETE
        public DateTime FechaOperacion { get; set; }
        public string? UsuarioOperacion { get; set; }
        
        // Propiedades formateadas
        public string FechaOperacionFormateada => FechaOperacion.ToString("dd/MMM/yyyy HH:mm");
        public string FechaPrimerPagoFormateada => FechaPrimerPago?.ToString("dd/MMM/yyyy") ?? "N/A";
        public string FechaFinalPagoFormateada => FechaFinalPago?.ToString("dd/MMM/yyyy") ?? "N/A";
    }

    public class UpdatePrestamoDto
    {
        [Required(ErrorMessage = "El ID del préstamo es requerido")]
        public int IdPrestamo { get; set; }

        [Required(ErrorMessage = "El empleado es requerido")]
        public int EmpleadoId { get; set; }

        [Required(ErrorMessage = "La cantidad total prestada es requerida")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
        public decimal CantidadTotalPrestada { get; set; }

        [Required(ErrorMessage = "La cantidad total a pagar es requerida")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad a pagar debe ser mayor a cero")]
        public decimal CantidadTotalAPagar { get; set; }

        [Required(ErrorMessage = "El interés aprobado es requerido")]
        [Range(0, 100, ErrorMessage = "El interés debe estar entre 0 y 100")]
        public decimal InteresAprobado { get; set; }

        [Required(ErrorMessage = "El interés moratorio es requerido")]
        [Range(0, 100, ErrorMessage = "El interés moratorio debe estar entre 0 y 100")]
        public decimal InteresMoratorio { get; set; }

        [Required(ErrorMessage = "El tipo de pago es requerido")]
        public int TipoPagoAbonoId { get; set; }

        public DateTime? FechaPrimerPago { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El total abonado a capital no puede ser negativo")]
        public decimal TotalAbonadoCapital { get; set; } = 0.00m;

        [Range(0, double.MaxValue, ErrorMessage = "El total abonado a intereses no puede ser negativo")]
        public decimal TotalAbonadoIntereses { get; set; } = 0.00m;

        public DateTime? FechaFinalPago { get; set; }

        [Required(ErrorMessage = "El autorizador es requerido")]
        public int AutorPersonaQueAutorizaId { get; set; }

        public string? Notas { get; set; }

        [Required(ErrorMessage = "El usuario de modificación es requerido")]
        public string UsuarioModificacion { get; set; } = string.Empty;
    }
}