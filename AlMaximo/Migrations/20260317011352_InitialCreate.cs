using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlMaximo.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumNomina = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellido2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoPagosAbonos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCorto = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoPagosAbonos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmpleadosAutorizadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpleadosAutorizadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpleadosAutorizadores_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prestamos",
                columns: table => new
                {
                    IdPrestamo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    CantidadTotalPrestada = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CantidadTotalAPagar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InteresAprobado = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    InteresMoratorio = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TipoPagoAbonoId = table.Column<int>(type: "int", nullable: false),
                    FechaPrimerPago = table.Column<DateTime>(type: "date", nullable: true),
                    TotalAbonadoCapital = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAbonadoIntereses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaFinalPago = table.Column<DateTime>(type: "date", nullable: true),
                    AutorPersonaQueAutorizaId = table.Column<int>(type: "int", nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UsuarioCreacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UsuarioModificacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prestamos", x => x.IdPrestamo);
                    table.CheckConstraint("CK_Prestamos_CantidadPositiva", "[CantidadTotalPrestada] > 0");
                    table.CheckConstraint("CK_Prestamos_InteresMoratorioPositivo", "[InteresMoratorio] >= 0");
                    table.CheckConstraint("CK_Prestamos_InteresPositivo", "[InteresAprobado] >= 0");
                    table.ForeignKey(
                        name: "FK_Prestamos_EmpleadosAutorizadores_AutorPersonaQueAutorizaId",
                        column: x => x.AutorPersonaQueAutorizaId,
                        principalTable: "EmpleadosAutorizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prestamos_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prestamos_TipoPagosAbonos_TipoPagoAbonoId",
                        column: x => x.TipoPagoAbonoId,
                        principalTable: "TipoPagosAbonos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrestamosHistorial",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrestamoId = table.Column<int>(type: "int", nullable: false),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    CantidadTotalPrestada = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CantidadTotalAPagar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InteresAprobado = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    InteresMoratorio = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TipoPagoAbonoId = table.Column<int>(type: "int", nullable: false),
                    FechaPrimerPago = table.Column<DateTime>(type: "date", nullable: true),
                    TotalAbonadoCapital = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAbonadoIntereses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Saldo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaFinalPago = table.Column<DateTime>(type: "date", nullable: true),
                    AutorPersonaQueAutorizaId = table.Column<int>(type: "int", nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    TipoOperacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaOperacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UsuarioOperacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrestamosHistorial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrestamosHistorial_EmpleadosAutorizadores_AutorPersonaQueAutorizaId",
                        column: x => x.AutorPersonaQueAutorizaId,
                        principalTable: "EmpleadosAutorizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrestamosHistorial_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrestamosHistorial_Prestamos_PrestamoId",
                        column: x => x.PrestamoId,
                        principalTable: "Prestamos",
                        principalColumn: "IdPrestamo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrestamosHistorial_TipoPagosAbonos_TipoPagoAbonoId",
                        column: x => x.TipoPagoAbonoId,
                        principalTable: "TipoPagosAbonos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_NumNomina",
                table: "Empleados",
                column: "NumNomina",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosAutorizadores_EmpleadoId",
                table: "EmpleadosAutorizadores",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_AutorPersonaQueAutorizaId",
                table: "Prestamos",
                column: "AutorPersonaQueAutorizaId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_EmpleadoId",
                table: "Prestamos",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_TipoPagoAbonoId",
                table: "Prestamos",
                column: "TipoPagoAbonoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrestamosHistorial_AutorPersonaQueAutorizaId",
                table: "PrestamosHistorial",
                column: "AutorPersonaQueAutorizaId");

            migrationBuilder.CreateIndex(
                name: "IX_PrestamosHistorial_EmpleadoId",
                table: "PrestamosHistorial",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrestamosHistorial_PrestamoId",
                table: "PrestamosHistorial",
                column: "PrestamoId");

            migrationBuilder.CreateIndex(
                name: "IX_PrestamosHistorial_TipoPagoAbonoId",
                table: "PrestamosHistorial",
                column: "TipoPagoAbonoId");

            migrationBuilder.CreateIndex(
                name: "IX_TipoPagosAbonos_NombreCorto",
                table: "TipoPagosAbonos",
                column: "NombreCorto",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrestamosHistorial");

            migrationBuilder.DropTable(
                name: "Prestamos");

            migrationBuilder.DropTable(
                name: "EmpleadosAutorizadores");

            migrationBuilder.DropTable(
                name: "TipoPagosAbonos");

            migrationBuilder.DropTable(
                name: "Empleados");
        }
    }
}
