USE AlMaximoLoansDB;
GO

-- Tabla de Empleados
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Empleados' AND xtype='U')
BEGIN
    CREATE TABLE Empleados (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        NumNomina NVARCHAR(20) NOT NULL UNIQUE,
        Nombres NVARCHAR(100) NOT NULL,
        Apellido1 NVARCHAR(50) NOT NULL,
        Apellido2 NVARCHAR(50) NULL,
        Activo BIT DEFAULT 1,
        FechaCreacion DATETIME2 DEFAULT GETDATE(),
        FechaModificacion DATETIME2 DEFAULT GETDATE()
    );
    PRINT 'Tabla Empleados creada exitosamente.';
END

-- Tabla de Tipos de Pago/Abonos
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TipoPagosAbonos' AND xtype='U')
BEGIN
    CREATE TABLE TipoPagosAbonos (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        NombreCorto NVARCHAR(20) NOT NULL UNIQUE,
        Descripcion NVARCHAR(100) NOT NULL,
        Activo BIT DEFAULT 1,
        FechaCreacion DATETIME2 DEFAULT GETDATE()
    );
    PRINT 'Tabla TipoPagosAbonos creada exitosamente.';
END

-- Tabla de Empleados Autorizadores
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EmpleadosAutorizadores' AND xtype='U')
BEGIN
    CREATE TABLE EmpleadosAutorizadores (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        EmpleadoId INT NOT NULL,
        Activo BIT DEFAULT 1,
        FechaCreacion DATETIME2 DEFAULT GETDATE(),
        CONSTRAINT FK_EmpleadosAutorizadores_Empleado FOREIGN KEY (EmpleadoId) REFERENCES Empleados(Id)
    );
    PRINT 'Tabla EmpleadosAutorizadores creada exitosamente.';
END

-- Tabla principal de Préstamos
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Prestamos' AND xtype='U')
BEGIN
    CREATE TABLE Prestamos (
        IdPrestamo INT IDENTITY(1,1) PRIMARY KEY,
        EmpleadoId INT NOT NULL,
        CantidadTotalPrestada DECIMAL(18,2) NOT NULL,
        CantidadTotalAPagar DECIMAL(18,2) NOT NULL,
        InteresAprobado DECIMAL(5,2) NOT NULL,
        InteresMoratorio DECIMAL(5,2) NOT NULL,
        TipoPagoAbonoId INT NOT NULL,
        FechaPrimerPago DATE NULL,
        TotalAbonadoCapital DECIMAL(18,2) DEFAULT 0.00,
        TotalAbonadoIntereses DECIMAL(18,2) DEFAULT 0.00,
        Saldo DECIMAL(18,2) NOT NULL,
        FechaFinalPago DATE NULL,
        AutorPersonaQueAutorizaId INT NOT NULL,
        Notas NVARCHAR(MAX) NULL,
        Activo BIT DEFAULT 1,
        FechaCreacion DATETIME2 DEFAULT GETDATE(),
        FechaModificacion DATETIME2 DEFAULT GETDATE(),
        UsuarioCreacion NVARCHAR(100) NULL,
        UsuarioModificacion NVARCHAR(100) NULL,
        
        CONSTRAINT FK_Prestamos_Empleado FOREIGN KEY (EmpleadoId) REFERENCES Empleados(Id),
        CONSTRAINT FK_Prestamos_TipoPagoAbono FOREIGN KEY (TipoPagoAbonoId) REFERENCES TipoPagosAbonos(Id),
        CONSTRAINT FK_Prestamos_Autorizador FOREIGN KEY (AutorPersonaQueAutorizaId) REFERENCES EmpleadosAutorizadores(Id),
        CONSTRAINT CK_Prestamos_CantidadPositiva CHECK (CantidadTotalPrestada > 0),
        CONSTRAINT CK_Prestamos_InteresPositivo CHECK (InteresAprobado >= 0),
        CONSTRAINT CK_Prestamos_InteresMoratorioPositivo CHECK (InteresMoratorio >= 0)
    );
    PRINT 'Tabla Prestamos creada exitosamente.';
END

-- Tabla de Historial de Cambios de Préstamos
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PrestamosHistorial' AND xtype='U')
BEGIN
    CREATE TABLE PrestamosHistorial (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PrestamoId INT NOT NULL,
        EmpleadoId INT NOT NULL,
        CantidadTotalPrestada DECIMAL(18,2) NOT NULL,
        CantidadTotalAPagar DECIMAL(18,2) NOT NULL,
        InteresAprobado DECIMAL(5,2) NOT NULL,
        InteresMoratorio DECIMAL(5,2) NOT NULL,
        TipoPagoAbonoId INT NOT NULL,
        FechaPrimerPago DATE NULL,
        TotalAbonadoCapital DECIMAL(18,2) NOT NULL,
        TotalAbonadoIntereses DECIMAL(18,2) NOT NULL,
        Saldo DECIMAL(18,2) NOT NULL,
        FechaFinalPago DATE NULL,
        AutorPersonaQueAutorizaId INT NOT NULL,
        Notas NVARCHAR(MAX) NULL,
        Activo BIT NOT NULL,
        TipoOperacion NVARCHAR(20) NOT NULL, -- 'INSERT', 'UPDATE', 'DELETE'
        FechaOperacion DATETIME2 DEFAULT GETDATE(),
        UsuarioOperacion NVARCHAR(100) NULL,
        
        CONSTRAINT FK_PrestamosHistorial_Prestamo FOREIGN KEY (PrestamoId) REFERENCES Prestamos(IdPrestamo),
        CONSTRAINT FK_PrestamosHistorial_Empleado FOREIGN KEY (EmpleadoId) REFERENCES Empleados(Id),
        CONSTRAINT FK_PrestamosHistorial_TipoPagoAbono FOREIGN KEY (TipoPagoAbonoId) REFERENCES TipoPagosAbonos(Id),
        CONSTRAINT FK_PrestamosHistorial_Autorizador FOREIGN KEY (AutorPersonaQueAutorizaId) REFERENCES EmpleadosAutorizadores(Id)
    );
    PRINT 'Tabla PrestamosHistorial creada exitosamente.';
END

-- Crear índices para mejorar el rendimiento
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Empleados_NumNomina')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Empleados_NumNomina ON Empleados(NumNomina);
    PRINT 'Índice IX_Empleados_NumNomina creado.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Prestamos_EmpleadoId')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Prestamos_EmpleadoId ON Prestamos(EmpleadoId);
    PRINT 'Índice IX_Prestamos_EmpleadoId creado.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PrestamosHistorial_PrestamoId')
BEGIN
    CREATE NONCLUSTERED INDEX IX_PrestamosHistorial_PrestamoId ON PrestamosHistorial(PrestamoId);
    PRINT 'Índice IX_PrestamosHistorial_PrestamoId creado.';
END

PRINT 'Script de creación de tablas ejecutado exitosamente.';