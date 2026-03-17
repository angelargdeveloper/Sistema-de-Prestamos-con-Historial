-- Script para insertar datos iniciales
-- Fecha: 16/mar/2026

USE AlMaximoLoansDB;
GO

-- Insertar tipos de pagos/abonos
IF NOT EXISTS (SELECT 1 FROM TipoPagosAbonos WHERE NombreCorto = 'SEMANAL')
BEGIN
    INSERT INTO TipoPagosAbonos (NombreCorto, Descripcion) VALUES 
    ('SEMANAL', 'Pago semanal cada 7 días'),
    ('CATORCENAL', 'Pago cada 14 días (dos semanas)'),
    ('QUINCENAL', 'Pago quincenal cada 15 días'),
    ('MENSUAL', 'Pago mensual cada 30 días'),
    ('ALFINAL', 'Pago único al final del periodo');
    PRINT 'Tipos de pagos/abonos insertados exitosamente.';
END
ELSE
BEGIN
    PRINT 'Los tipos de pagos/abonos ya existen.';
END

-- Insertar empleados de ejemplo
IF NOT EXISTS (SELECT 1 FROM Empleados WHERE NumNomina = 'EMP001')
BEGIN
    INSERT INTO Empleados (NumNomina, Nombres, Apellido1, Apellido2) VALUES 
    ('EMP001', 'Juan Carlos', 'García', 'López'),
    ('EMP002', 'María Elena', 'Rodríguez', 'Martínez'),
    ('EMP003', 'Luis Fernando', 'Hernández', 'Díaz'),
    ('EMP004', 'Ana Sofía', 'González', 'Ruiz'),
    ('EMP005', 'Carlos Alberto', 'Jiménez', 'Torres'),
    ('EMP006', 'Diana Elizabeth', 'Cruz', 'Morales'),
    ('EMP007', 'Roberto Miguel', 'Vargas', 'Castillo'),
    ('EMP008', 'Patricia Isabel', 'Mendoza', 'Flores'),
    ('EMP009', 'Alejandro José', 'Ramírez', 'Silva'),
    ('EMP010', 'Gabriela Andrea', 'Ortega', 'Guerrero');
    PRINT 'Empleados de ejemplo insertados exitosamente.';
END
ELSE
BEGIN
    PRINT 'Los empleados de ejemplo ya existen.';
END

-- Insertar empleados autorizadores (Los primeros 5 empleados serán autorizadores)
IF NOT EXISTS (SELECT 1 FROM EmpleadosAutorizadores)
BEGIN
    INSERT INTO EmpleadosAutorizadores (EmpleadoId)
    SELECT Id FROM Empleados WHERE Id <= 5;
    PRINT 'Empleados autorizadores insertados exitosamente.';
END
ELSE
BEGIN
    PRINT 'Los empleados autorizadores ya existen.';
END

-- Insertar algunos préstamos de ejemplo
IF NOT EXISTS (SELECT 1 FROM Prestamos)
BEGIN
    INSERT INTO Prestamos (
        EmpleadoId, 
        CantidadTotalPrestada, 
        CantidadTotalAPagar, 
        InteresAprobado, 
        InteresMoratorio, 
        TipoPagoAbonoId, 
        FechaPrimerPago, 
        Saldo, 
        FechaFinalPago, 
        AutorPersonaQueAutorizaId, 
        Notas,
        UsuarioCreacion
    ) VALUES 
    (6, 50000.00, 55000.00, 10.00, 2.00, 4, '2026-04-15', 55000.00, '2027-04-15', 1, 'Préstamo para gastos médicos', 'SYSTEM'),
    (7, 25000.00, 26875.00, 7.50, 1.50, 3, '2026-03-30', 26875.00, '2026-12-30', 2, 'Préstamo para educación', 'SYSTEM'),
    (8, 75000.00, 82500.00, 10.00, 2.50, 4, '2026-04-01', 82500.00, '2027-04-01', 3, 'Préstamo para vivienda', 'SYSTEM');
    PRINT 'Préstamos de ejemplo insertados exitosamente.';
END
ELSE
BEGIN
    PRINT 'Los préstamos de ejemplo ya existen.';
END

PRINT 'Script de datos iniciales ejecutado exitosamente.';