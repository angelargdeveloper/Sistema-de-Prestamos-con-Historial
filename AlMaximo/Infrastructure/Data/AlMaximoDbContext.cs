using Microsoft.EntityFrameworkCore;
using AlMaximo.Core.Entities;

namespace AlMaximo.Infrastructure.Data
{
    public class AlMaximoDbContext : DbContext
    {
        public AlMaximoDbContext(DbContextOptions<AlMaximoDbContext> options) : base(options)
        {
        }

        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<TipoPagoAbono> TipoPagosAbonos { get; set; }
        public DbSet<EmpleadoAutorizador> EmpleadosAutorizadores { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }
        public DbSet<PrestamoHistorial> PrestamosHistorial { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar relaciones
            modelBuilder.Entity<EmpleadoAutorizador>()
                .HasOne(ea => ea.Empleado)
                .WithMany(e => e.EmpleadoAutorizadores)
                .HasForeignKey(ea => ea.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Empleado)
                .WithMany(e => e.Prestamos)
                .HasForeignKey(p => p.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.TipoPagoAbono)
                .WithMany(tpa => tpa.Prestamos)
                .HasForeignKey(p => p.TipoPagoAbonoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.AutorPersonaQueAutoriza)
                .WithMany(ea => ea.PrestamosAutorizados)
                .HasForeignKey(p => p.AutorPersonaQueAutorizaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PrestamoHistorial>()
                .HasOne(ph => ph.Prestamo)
                .WithMany(p => p.HistorialCambios)
                .HasForeignKey(ph => ph.PrestamoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PrestamoHistorial>()
                .HasOne(ph => ph.Empleado)
                .WithMany()
                .HasForeignKey(ph => ph.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PrestamoHistorial>()
                .HasOne(ph => ph.TipoPagoAbono)
                .WithMany()
                .HasForeignKey(ph => ph.TipoPagoAbonoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PrestamoHistorial>()
                .HasOne(ph => ph.AutorPersonaQueAutoriza)
                .WithMany()
                .HasForeignKey(ph => ph.AutorPersonaQueAutorizaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configurar índices únicos
            modelBuilder.Entity<Empleado>()
                .HasIndex(e => e.NumNomina)
                .IsUnique();

            modelBuilder.Entity<TipoPagoAbono>()
                .HasIndex(tpa => tpa.NombreCorto)
                .IsUnique();

            // Configurar valores por defecto
            modelBuilder.Entity<Empleado>()
                .Property(e => e.Activo)
                .HasDefaultValue(true);

            modelBuilder.Entity<Empleado>()
                .Property(e => e.FechaCreacion)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Empleado>()
                .Property(e => e.FechaModificacion)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<TipoPagoAbono>()
                .Property(tpa => tpa.Activo)
                .HasDefaultValue(true);

            modelBuilder.Entity<TipoPagoAbono>()
                .Property(tpa => tpa.FechaCreacion)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<EmpleadoAutorizador>()
                .Property(ea => ea.Activo)
                .HasDefaultValue(true);

            modelBuilder.Entity<EmpleadoAutorizador>()
                .Property(ea => ea.FechaCreacion)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Prestamo>()
                .Property(p => p.Activo)
                .HasDefaultValue(true);

            modelBuilder.Entity<Prestamo>()
                .Property(p => p.FechaCreacion)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Prestamo>()
                .Property(p => p.FechaModificacion)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<PrestamoHistorial>()
                .Property(ph => ph.FechaOperacion)
                .HasDefaultValueSql("GETDATE()");

            // Configurar restricciones Check (a través de SQL bruto si es necesario)
            modelBuilder.Entity<Prestamo>()
                .ToTable(t => t.HasCheckConstraint("CK_Prestamos_CantidadPositiva", "[CantidadTotalPrestada] > 0"));

            modelBuilder.Entity<Prestamo>()
                .ToTable(t => t.HasCheckConstraint("CK_Prestamos_InteresPositivo", "[InteresAprobado] >= 0"));

            modelBuilder.Entity<Prestamo>()
                .ToTable(t => t.HasCheckConstraint("CK_Prestamos_InteresMoratorioPositivo", "[InteresMoratorio] >= 0"));
        }
    }
}