USE master;
GO

-- Crear la base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'AlMaximoLoansDB')
BEGIN
    CREATE DATABASE AlMaximoLoansDB;
    PRINT 'Base de datos AlMaximoLoansDB creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La base de datos AlMaximoLoansDB ya existe.';
END
GO

USE AlMaximoLoansDB;
GO

PRINT 'Usando base de datos AlMaximoLoansDB';