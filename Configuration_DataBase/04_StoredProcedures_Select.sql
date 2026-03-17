-- Procedimientos almacenados para la gestión de préstamos
-- Fecha: 16/mar/2026

USE AlMaximoLoansDB;
GO

-- =============================================
-- Procedimiento para obtener todos los empleados (para búsqueda eficiente)
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_GetEmpleados')
    DROP PROCEDURE SP_GetEmpleados;
GO

CREATE PROCEDURE SP_GetEmpleados
    @Busqueda NVARCHAR(100) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        Id,
        NumNomina,
        Nombres,
        Apellido1,
        Apellido2,
        (Nombres + ' ' + Apellido1 + ISNULL(' ' + Apellido2, '')) AS NombreCompleto,
        Activo
    FROM Empleados
    WHERE Activo = 1
        AND (@Busqueda IS NULL OR 
             Nombres LIKE '%' + @Busqueda + '%' OR
             Apellido1 LIKE '%' + @Busqueda + '%' OR
             Apellido2 LIKE '%' + @Busqueda + '%' OR
             NumNomina LIKE '%' + @Busqueda + '%')
    ORDER BY Apellido1, Apellido2, Nombres
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
    
    -- Obtener el total de registros
    SELECT COUNT(*) AS TotalRegistros
    FROM Empleados
    WHERE Activo = 1
        AND (@Busqueda IS NULL OR 
             Nombres LIKE '%' + @Busqueda + '%' OR
             Apellido1 LIKE '%' + @Busqueda + '%' OR
             Apellido2 LIKE '%' + @Busqueda + '%' OR
             NumNomina LIKE '%' + @Busqueda + '%');
END;
GO

-- =============================================
-- Procedimiento para obtener empleados autorizadores
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_GetEmpleadosAutorizadores')
    DROP PROCEDURE SP_GetEmpleadosAutorizadores;
GO

CREATE PROCEDURE SP_GetEmpleadosAutorizadores
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ea.Id,
        e.NumNomina,
        (e.Nombres + ' ' + e.Apellido1 + ISNULL(' ' + e.Apellido2, '')) AS NombreCompleto
    FROM EmpleadosAutorizadores ea
    INNER JOIN Empleados e ON ea.EmpleadoId = e.Id
    WHERE ea.Activo = 1 AND e.Activo = 1
    ORDER BY e.Apellido1, e.Apellido2, e.Nombres;
END;
GO

-- =============================================
-- Procedimiento para obtener tipos de pago
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_GetTiposPagoAbono')
    DROP PROCEDURE SP_GetTiposPagoAbono;
GO

CREATE PROCEDURE SP_GetTiposPagoAbono
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        NombreCorto,
        Descripcion
    FROM TipoPagosAbonos
    WHERE Activo = 1
    ORDER BY Id;
END;
GO

-- =============================================
-- Procedimiento para obtener préstamos con filtros
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_GetPrestamos')
    DROP PROCEDURE SP_GetPrestamos;
GO

CREATE PROCEDURE SP_GetPrestamos
    @Busqueda NVARCHAR(100) = NULL,
    @EmpleadoId INT = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        p.IdPrestamo,
        p.EmpleadoId,
        (e.Nombres + ' ' + e.Apellido1 + ISNULL(' ' + e.Apellido2, '')) AS NombreEmpleado,
        e.NumNomina,
        p.CantidadTotalPrestada,
        p.CantidadTotalAPagar,
        p.InteresAprobado,
        p.InteresMoratorio,
        p.TipoPagoAbonoId,
        tpa.NombreCorto AS TipoPago,
        tpa.Descripcion AS DescripcionTipoPago,
        p.FechaPrimerPago,
        p.TotalAbonadoCapital,
        p.TotalAbonadoIntereses,
        p.Saldo,
        p.FechaFinalPago,
        p.AutorPersonaQueAutorizaId,
        (ea.Nombres + ' ' + ea.Apellido1 + ISNULL(' ' + ea.Apellido2, '')) AS NombreAutorizador,
        p.Notas,
        p.Activo,
        p.FechaCreacion,
        p.FechaModificacion,
        p.UsuarioCreacion,
        p.UsuarioModificacion
    FROM Prestamos p
    INNER JOIN Empleados e ON p.EmpleadoId = e.Id
    INNER JOIN TipoPagosAbonos tpa ON p.TipoPagoAbonoId = tpa.Id
    INNER JOIN EmpleadosAutorizadores eauth ON p.AutorPersonaQueAutorizaId = eauth.Id
    INNER JOIN Empleados ea ON eauth.EmpleadoId = ea.Id
    WHERE p.Activo = 1
        AND (@EmpleadoId IS NULL OR p.EmpleadoId = @EmpleadoId)
        AND (@Busqueda IS NULL OR 
             e.Nombres LIKE '%' + @Busqueda + '%' OR
             e.Apellido1 LIKE '%' + @Busqueda + '%' OR
             e.Apellido2 LIKE '%' + @Busqueda + '%' OR
             e.NumNomina LIKE '%' + @Busqueda + '%' OR
             CAST(p.IdPrestamo AS NVARCHAR) LIKE '%' + @Busqueda + '%')
    ORDER BY p.FechaCreacion DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
    
    -- Obtener el total de registros
    SELECT COUNT(*) AS TotalRegistros
    FROM Prestamos p
    INNER JOIN Empleados e ON p.EmpleadoId = e.Id
    WHERE p.Activo = 1
        AND (@EmpleadoId IS NULL OR p.EmpleadoId = @EmpleadoId)
        AND (@Busqueda IS NULL OR 
             e.Nombres LIKE '%' + @Busqueda + '%' OR
             e.Apellido1 LIKE '%' + @Busqueda + '%' OR
             e.Apellido2 LIKE '%' + @Busqueda + '%' OR
             e.NumNomina LIKE '%' + @Busqueda + '%' OR
             CAST(p.IdPrestamo AS NVARCHAR) LIKE '%' + @Busqueda + '%');
END;
GO

-- =============================================
-- Procedimiento para obtener un préstamo por ID
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_GetPrestamoById')
    DROP PROCEDURE SP_GetPrestamoById;
GO

CREATE PROCEDURE SP_GetPrestamoById
    @IdPrestamo INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.IdPrestamo,
        p.EmpleadoId,
        (e.Nombres + ' ' + e.Apellido1 + ISNULL(' ' + e.Apellido2, '')) AS NombreEmpleado,
        e.NumNomina,
        p.CantidadTotalPrestada,
        p.CantidadTotalAPagar,
        p.InteresAprobado,
        p.InteresMoratorio,
        p.TipoPagoAbonoId,
        tpa.NombreCorto AS TipoPago,
        tpa.Descripcion AS DescripcionTipoPago,
        p.FechaPrimerPago,
        p.TotalAbonadoCapital,
        p.TotalAbonadoIntereses,
        p.Saldo,
        p.FechaFinalPago,
        p.AutorPersonaQueAutorizaId,
        (ea.Nombres + ' ' + ea.Apellido1 + ISNULL(' ' + ea.Apellido2, '')) AS NombreAutorizador,
        p.Notas,
        p.Activo,
        p.FechaCreacion,
        p.FechaModificacion,
        p.UsuarioCreacion,
        p.UsuarioModificacion
    FROM Prestamos p
    INNER JOIN Empleados e ON p.EmpleadoId = e.Id
    INNER JOIN TipoPagosAbonos tpa ON p.TipoPagoAbonoId = tpa.Id
    INNER JOIN EmpleadosAutorizadores eauth ON p.AutorPersonaQueAutorizaId = eauth.Id
    INNER JOIN Empleados ea ON eauth.EmpleadoId = ea.Id
    WHERE p.IdPrestamo = @IdPrestamo AND p.Activo = 1;
END;
GO

PRINT 'Procedimientos almacenados de consulta creados exitosamente.';