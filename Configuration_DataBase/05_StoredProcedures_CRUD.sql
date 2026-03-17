-- Procedimientos almacenados para operaciones CRUD de préstamos
-- Fecha: 16/mar/2026

USE AlMaximoLoansDB;
GO

-- =============================================
-- Procedimiento para crear un préstamo
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_CreatePrestamo')
    DROP PROCEDURE SP_CreatePrestamo;
GO

CREATE PROCEDURE SP_CreatePrestamo
    @EmpleadoId INT,
    @CantidadTotalPrestada DECIMAL(18,2),
    @CantidadTotalAPagar DECIMAL(18,2),
    @InteresAprobado DECIMAL(5,2),
    @InteresMoratorio DECIMAL(5,2),
    @TipoPagoAbonoId INT,
    @FechaPrimerPago DATE = NULL,
    @FechaFinalPago DATE = NULL,
    @AutorPersonaQueAutorizaId INT,
    @Notas NVARCHAR(MAX) = NULL,
    @UsuarioCreacion NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validaciones
        IF NOT EXISTS (SELECT 1 FROM Empleados WHERE Id = @EmpleadoId AND Activo = 1)
        BEGIN
            RAISERROR('El empleado especificado no existe o no está activo.', 16, 1);
            RETURN;
        END
        
        IF NOT EXISTS (SELECT 1 FROM TipoPagosAbonos WHERE Id = @TipoPagoAbonoId AND Activo = 1)
        BEGIN
            RAISERROR('El tipo de pago especificado no existe o no está activo.', 16, 1);
            RETURN;
        END
        
        IF NOT EXISTS (SELECT 1 FROM EmpleadosAutorizadores WHERE Id = @AutorPersonaQueAutorizaId AND Activo = 1)
        BEGIN
            RAISERROR('El empleado autorizador especificado no existe o no está activo.', 16, 1);
            RETURN;
        END
        
        -- El saldo inicial es igual a la cantidad total a pagar
        DECLARE @Saldo DECIMAL(18,2) = @CantidadTotalAPagar;
        
        -- Insertar el préstamo
        INSERT INTO Prestamos (
            EmpleadoId, 
            CantidadTotalPrestada, 
            CantidadTotalAPagar, 
            InteresAprobado, 
            InteresMoratorio, 
            TipoPagoAbonoId, 
            FechaPrimerPago, 
            TotalAbonadoCapital, 
            TotalAbonadoIntereses, 
            Saldo, 
            FechaFinalPago, 
            AutorPersonaQueAutorizaId, 
            Notas,
            UsuarioCreacion,
            UsuarioModificacion
        )
        VALUES (
            @EmpleadoId,
            @CantidadTotalPrestada,
            @CantidadTotalAPagar,
            @InteresAprobado,
            @InteresMoratorio,
            @TipoPagoAbonoId,
            @FechaPrimerPago,
            0.00,
            0.00,
            @Saldo,
            @FechaFinalPago,
            @AutorPersonaQueAutorizaId,
            @Notas,
            @UsuarioCreacion,
            @UsuarioCreacion
        );
        
        DECLARE @IdPrestamoCreado INT = SCOPE_IDENTITY();
        
        -- Insertar en el historial
        INSERT INTO PrestamosHistorial (
            PrestamoId,
            EmpleadoId, 
            CantidadTotalPrestada, 
            CantidadTotalAPagar, 
            InteresAprobado, 
            InteresMoratorio, 
            TipoPagoAbonoId, 
            FechaPrimerPago, 
            TotalAbonadoCapital, 
            TotalAbonadoIntereses, 
            Saldo, 
            FechaFinalPago, 
            AutorPersonaQueAutorizaId, 
            Notas,
            Activo,
            TipoOperacion,
            UsuarioOperacion
        )
        VALUES (
            @IdPrestamoCreado,
            @EmpleadoId,
            @CantidadTotalPrestada,
            @CantidadTotalAPagar,
            @InteresAprobado,
            @InteresMoratorio,
            @TipoPagoAbonoId,
            @FechaPrimerPago,
            0.00,
            0.00,
            @Saldo,
            @FechaFinalPago,
            @AutorPersonaQueAutorizaId,
            @Notas,
            1,
            'INSERT',
            @UsuarioCreacion
        );
        
        COMMIT TRANSACTION;
        
        -- Devolver el ID del préstamo creado
        SELECT @IdPrestamoCreado AS IdPrestamo;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

-- =============================================
-- Procedimiento para actualizar un préstamo
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_UpdatePrestamo')
    DROP PROCEDURE SP_UpdatePrestamo;
GO

CREATE PROCEDURE SP_UpdatePrestamo
    @IdPrestamo INT,
    @EmpleadoId INT,
    @CantidadTotalPrestada DECIMAL(18,2),
    @CantidadTotalAPagar DECIMAL(18,2),
    @InteresAprobado DECIMAL(5,2),
    @InteresMoratorio DECIMAL(5,2),
    @TipoPagoAbonoId INT,
    @FechaPrimerPago DATE = NULL,
    @TotalAbonadoCapital DECIMAL(18,2) = 0.00,
    @TotalAbonadoIntereses DECIMAL(18,2) = 0.00,
    @FechaFinalPago DATE = NULL,
    @AutorPersonaQueAutorizaId INT,
    @Notas NVARCHAR(MAX) = NULL,
    @UsuarioModificacion NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar que el préstamo existe
        IF NOT EXISTS (SELECT 1 FROM Prestamos WHERE IdPrestamo = @IdPrestamo AND Activo = 1)
        BEGIN
            RAISERROR('El préstamo especificado no existe o no está activo.', 16, 1);
            RETURN;
        END
        
        -- Validaciones
        IF NOT EXISTS (SELECT 1 FROM Empleados WHERE Id = @EmpleadoId AND Activo = 1)
        BEGIN
            RAISERROR('El empleado especificado no existe o no está activo.', 16, 1);
            RETURN;
        END
        
        IF NOT EXISTS (SELECT 1 FROM TipoPagosAbonos WHERE Id = @TipoPagoAbonoId AND Activo = 1)
        BEGIN
            RAISERROR('El tipo de pago especificado no existe o no está activo.', 16, 1);
            RETURN;
        END
        
        IF NOT EXISTS (SELECT 1 FROM EmpleadosAutorizadores WHERE Id = @AutorPersonaQueAutorizaId AND Activo = 1)
        BEGIN
            RAISERROR('El empleado autorizador especificado no existe o no está activo.', 16, 1);
            RETURN;
        END
        
        -- Calcular el nuevo saldo
        DECLARE @NuevoSaldo DECIMAL(18,2) = @CantidadTotalAPagar - (@TotalAbonadoCapital + @TotalAbonadoIntereses);
        
        -- Actualizar el préstamo
        UPDATE Prestamos SET
            EmpleadoId = @EmpleadoId,
            CantidadTotalPrestada = @CantidadTotalPrestada,
            CantidadTotalAPagar = @CantidadTotalAPagar,
            InteresAprobado = @InteresAprobado,
            InteresMoratorio = @InteresMoratorio,
            TipoPagoAbonoId = @TipoPagoAbonoId,
            FechaPrimerPago = @FechaPrimerPago,
            TotalAbonadoCapital = @TotalAbonadoCapital,
            TotalAbonadoIntereses = @TotalAbonadoIntereses,
            Saldo = @NuevoSaldo,
            FechaFinalPago = @FechaFinalPago,
            AutorPersonaQueAutorizaId = @AutorPersonaQueAutorizaId,
            Notas = @Notas,
            FechaModificacion = GETDATE(),
            UsuarioModificacion = @UsuarioModificacion
        WHERE IdPrestamo = @IdPrestamo;
        
        -- Insertar en el historial
        INSERT INTO PrestamosHistorial (
            PrestamoId,
            EmpleadoId, 
            CantidadTotalPrestada, 
            CantidadTotalAPagar, 
            InteresAprobado, 
            InteresMoratorio, 
            TipoPagoAbonoId, 
            FechaPrimerPago, 
            TotalAbonadoCapital, 
            TotalAbonadoIntereses, 
            Saldo, 
            FechaFinalPago, 
            AutorPersonaQueAutorizaId, 
            Notas,
            Activo,
            TipoOperacion,
            UsuarioOperacion
        )
        VALUES (
            @IdPrestamo,
            @EmpleadoId,
            @CantidadTotalPrestada,
            @CantidadTotalAPagar,
            @InteresAprobado,
            @InteresMoratorio,
            @TipoPagoAbonoId,
            @FechaPrimerPago,
            @TotalAbonadoCapital,
            @TotalAbonadoIntereses,
            @NuevoSaldo,
            @FechaFinalPago,
            @AutorPersonaQueAutorizaId,
            @Notas,
            1,
            'UPDATE',
            @UsuarioModificacion
        );
        
        COMMIT TRANSACTION;
        
        SELECT 'Préstamo actualizado exitosamente.' AS Mensaje;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

-- =============================================
-- Procedimiento para eliminar (desactivar) un préstamo
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SP_DeletePrestamo')
    DROP PROCEDURE SP_DeletePrestamo;
GO

CREATE PROCEDURE SP_DeletePrestamo
    @IdPrestamo INT,
    @UsuarioEliminacion NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar que el préstamo existe
        IF NOT EXISTS (SELECT 1 FROM Prestamos WHERE IdPrestamo = @IdPrestamo AND Activo = 1)
        BEGIN
            RAISERROR('El préstamo especificado no existe o ya está inactivo.', 16, 1);
            RETURN;
        END
        
        -- Obtener los datos actuales del préstamo antes de desactivarlo
        DECLARE @EmpleadoId INT, @CantidadTotalPrestada DECIMAL(18,2), @CantidadTotalAPagar DECIMAL(18,2),
                @InteresAprobado DECIMAL(5,2), @InteresMoratorio DECIMAL(5,2), @TipoPagoAbonoId INT,
                @FechaPrimerPago DATE, @TotalAbonadoCapital DECIMAL(18,2), @TotalAbonadoIntereses DECIMAL(18,2),
                @Saldo DECIMAL(18,2), @FechaFinalPago DATE, @AutorPersonaQueAutorizaId INT, @Notas NVARCHAR(MAX);
        
        SELECT 
            @EmpleadoId = EmpleadoId,
            @CantidadTotalPrestada = CantidadTotalPrestada,
            @CantidadTotalAPagar = CantidadTotalAPagar,
            @InteresAprobado = InteresAprobado,
            @InteresMoratorio = InteresMoratorio,
            @TipoPagoAbonoId = TipoPagoAbonoId,
            @FechaPrimerPago = FechaPrimerPago,
            @TotalAbonadoCapital = TotalAbonadoCapital,
            @TotalAbonadoIntereses = TotalAbonadoIntereses,
            @Saldo = Saldo,
            @FechaFinalPago = FechaFinalPago,
            @AutorPersonaQueAutorizaId = AutorPersonaQueAutorizaId,
            @Notas = Notas
        FROM Prestamos 
        WHERE IdPrestamo = @IdPrestamo;
        
        -- Desactivar el préstamo (eliminación lógica)
        UPDATE Prestamos SET
            Activo = 0,
            FechaModificacion = GETDATE(),
            UsuarioModificacion = @UsuarioEliminacion
        WHERE IdPrestamo = @IdPrestamo;
        
        -- Insertar en el historial
        INSERT INTO PrestamosHistorial (
            PrestamoId,
            EmpleadoId, 
            CantidadTotalPrestada, 
            CantidadTotalAPagar, 
            InteresAprobado, 
            InteresMoratorio, 
            TipoPagoAbonoId, 
            FechaPrimerPago, 
            TotalAbonadoCapital, 
            TotalAbonadoIntereses, 
            Saldo, 
            FechaFinalPago, 
            AutorPersonaQueAutorizaId, 
            Notas,
            Activo,
            TipoOperacion,
            UsuarioOperacion
        )
        VALUES (
            @IdPrestamo,
            @EmpleadoId,
            @CantidadTotalPrestada,
            @CantidadTotalAPagar,
            @InteresAprobado,
            @InteresMoratorio,
            @TipoPagoAbonoId,
            @FechaPrimerPago,
            @TotalAbonadoCapital,
            @TotalAbonadoIntereses,
            @Saldo,
            @FechaFinalPago,
            @AutorPersonaQueAutorizaId,
            @Notas,
            0,
            'DELETE',
            @UsuarioEliminacion
        );
        
        COMMIT TRANSACTION;
        
        SELECT 'Préstamo eliminado exitosamente.' AS Mensaje;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO

PRINT 'Procedimientos almacenados CRUD creados exitosamente.';