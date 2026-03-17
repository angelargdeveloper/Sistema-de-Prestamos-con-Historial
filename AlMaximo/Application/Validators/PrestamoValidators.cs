using FluentValidation;
using AlMaximo.Core.DTOs;

namespace AlMaximo.Application.Validators
{
    public class CreatePrestamoValidator : AbstractValidator<CreatePrestamoDto>
    {
        public CreatePrestamoValidator()
        {
            RuleFor(x => x.EmpleadoId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un empleado válido.");

            RuleFor(x => x.CantidadTotalPrestada)
                .GreaterThan(0)
                .WithMessage("La cantidad total prestada debe ser mayor a cero.")
                .LessThanOrEqualTo(1000000)
                .WithMessage("La cantidad total prestada no puede exceder $1,000,000.00");

            RuleFor(x => x.CantidadTotalAPagar)
                .GreaterThan(0)
                .WithMessage("La cantidad total a pagar debe ser mayor a cero.")
                .GreaterThanOrEqualTo(x => x.CantidadTotalPrestada)
                .WithMessage("La cantidad total a pagar debe ser mayor o igual a la cantidad prestada.");

            RuleFor(x => x.InteresAprobado)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El interés aprobado no puede ser negativo.")
                .LessThanOrEqualTo(100)
                .WithMessage("El interés aprobado no puede ser mayor al 100%.");

            RuleFor(x => x.InteresMoratorio)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El interés moratorio no puede ser negativo.")
                .LessThanOrEqualTo(100)
                .WithMessage("El interés moratorio no puede ser mayor al 100%.");

            RuleFor(x => x.TipoPagoAbonoId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un tipo de pago válido.");

            RuleFor(x => x.FechaPrimerPago)
                .Must(BeValidFirstPaymentDate)
                .WithMessage("La fecha del primer pago debe ser futura (excepto para tipo 'Al Final').")
                .When(x => x.FechaPrimerPago.HasValue);

            RuleFor(x => x.FechaFinalPago)
                .Must((dto, fechaFinal) => BeValidFinalPaymentDate(dto, fechaFinal))
                .WithMessage("La fecha final de pago debe ser posterior a la fecha del primer pago.")
                .When(x => x.FechaFinalPago.HasValue && x.FechaPrimerPago.HasValue);

            RuleFor(x => x.AutorPersonaQueAutorizaId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar una persona autorizadora válida.");

            RuleFor(x => x.Notas)
                .MaximumLength(1000)
                .WithMessage("Las notas no pueden exceder 1000 caracteres.");

            RuleFor(x => x.UsuarioCreacion)
                .NotEmpty()
                .WithMessage("El usuario de creación es requerido.")
                .MaximumLength(100)
                .WithMessage("El usuario de creación no puede exceder 100 caracteres.");
        }

        private bool BeValidFirstPaymentDate(DateTime? fechaPrimerPago)
        {
            if (!fechaPrimerPago.HasValue)
                return true; // Para tipo "Al Final" puede ser null

            return fechaPrimerPago.Value.Date >= DateTime.Today;
        }

        private bool BeValidFinalPaymentDate(CreatePrestamoDto dto, DateTime? fechaFinal)
        {
            if (!fechaFinal.HasValue || !dto.FechaPrimerPago.HasValue)
                return true;

            return fechaFinal.Value.Date > dto.FechaPrimerPago.Value.Date;
        }
    }

    public class UpdatePrestamoValidator : AbstractValidator<UpdatePrestamoDto>
    {
        public UpdatePrestamoValidator()
        {
            RuleFor(x => x.IdPrestamo)
                .GreaterThan(0)
                .WithMessage("El ID del préstamo debe ser válido.");

            RuleFor(x => x.EmpleadoId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un empleado válido.");

            RuleFor(x => x.CantidadTotalPrestada)
                .GreaterThan(0)
                .WithMessage("La cantidad total prestada debe ser mayor a cero.")
                .LessThanOrEqualTo(1000000)
                .WithMessage("La cantidad total prestada no puede exceder $1,000,000.00");

            RuleFor(x => x.CantidadTotalAPagar)
                .GreaterThan(0)
                .WithMessage("La cantidad total a pagar debe ser mayor a cero.")
                .GreaterThanOrEqualTo(x => x.CantidadTotalPrestada)
                .WithMessage("La cantidad total a pagar debe ser mayor o igual a la cantidad prestada.");

            RuleFor(x => x.InteresAprobado)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El interés aprobado no puede ser negativo.")
                .LessThanOrEqualTo(100)
                .WithMessage("El interés aprobado no puede ser mayor al 100%.");

            RuleFor(x => x.InteresMoratorio)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El interés moratorio no puede ser negativo.")
                .LessThanOrEqualTo(100)
                .WithMessage("El interés moratorio no puede ser mayor al 100%.");

            RuleFor(x => x.TipoPagoAbonoId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un tipo de pago válido.");

            RuleFor(x => x.TotalAbonadoCapital)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El total abonado a capital no puede ser negativo.")
                .LessThanOrEqualTo(x => x.CantidadTotalPrestada)
                .WithMessage("El total abonado a capital no puede ser mayor a la cantidad prestada.");

            RuleFor(x => x.TotalAbonadoIntereses)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El total abonado a intereses no puede ser negativo.");

            RuleFor(x => x)
                .Must(BeValidTotalPayments)
                .WithMessage("La suma de los abonos no puede exceder la cantidad total a pagar.");

            RuleFor(x => x.AutorPersonaQueAutorizaId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar una persona autorizadora válida.");

            RuleFor(x => x.Notas)
                .MaximumLength(1000)
                .WithMessage("Las notas no pueden exceder 1000 caracteres.");

            RuleFor(x => x.UsuarioModificacion)
                .NotEmpty()
                .WithMessage("El usuario de modificación es requerido.")
                .MaximumLength(100)
                .WithMessage("El usuario de modificación no puede exceder 100 caracteres.");
        }

        private bool BeValidTotalPayments(UpdatePrestamoDto dto)
        {
            var totalAbonado = dto.TotalAbonadoCapital + dto.TotalAbonadoIntereses;
            return totalAbonado <= dto.CantidadTotalAPagar;
        }
    }

    public class SearchParametersValidator : AbstractValidator<SearchParametersDto>
    {
        public SearchParametersValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("El número de página debe ser mayor a cero.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("El tamaño de página debe estar entre 1 y 100.");

            RuleFor(x => x.Busqueda)
                .MaximumLength(100)
                .WithMessage("El término de búsqueda no puede exceder 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Busqueda));
        }
    }
}