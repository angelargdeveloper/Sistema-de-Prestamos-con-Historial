namespace AlMaximo.Core.DTOs
{
    public class EmpleadoDto
    {
        public int Id { get; set; }
        public string NumNomina { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellido1 { get; set; } = string.Empty;
        public string? Apellido2 { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }

    public class EmpleadoAutorizadorDto
    {
        public int Id { get; set; }
        public string NumNomina { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
    }

    public class TipoPagoAbonoDto
    {
        public int Id { get; set; }
        public string NombreCorto { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}