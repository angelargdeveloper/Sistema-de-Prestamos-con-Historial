-- Script principal para ejecutar todos los scripts de base de datos
-- Fecha: 16/mar/2026
-- IMPORTANTE: Ejecutar cada script en orden

-- Instrucciones:
-- 1. Abrir SQL Server Management Studio
-- 2. Conectarse a su instancia de SQL Server
-- 3. Ejecutar cada script en el orden especificado

/*
ORDEN DE EJECUCIÓN:

1. 01_CreateDatabase.sql
   - Crea la base de datos AlMaximoLoansDB

2. 02_CreateTables.sql
   - Crea todas las tablas del sistema
   - Crea índices para optimizar el rendimiento

3. 03_InsertInitialData.sql
   - Inserta datos iniciales (catálogos y ejemplos)

4. 04_StoredProcedures_Select.sql
   - Crea procedimientos almacenados para consultas

5. 05_StoredProcedures_CRUD.sql
   - Crea procedimientos almacenados para operaciones CRUD

NOTA: Todos los scripts están diseñados para ser idempotentes,
es decir, se pueden ejecutar múltiples veces sin causar errores.
*/

-- Verificar versión de SQL Server (debe ser 2017 o superior)
SELECT @@VERSION as 'SQL Server Version';

-- Verificar que la base de datos se creó correctamente
USE AlMaximoLoansDB;
GO

SELECT 'Base de datos AlMaximoLoansDB configurada correctamente.' AS Mensaje;

-- Verificar las tablas creadas
SELECT 
    TABLE_NAME as 'Tablas Creadas'
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- Verificar los procedimientos almacenados creados
SELECT 
    ROUTINE_NAME as 'Procedimientos Almacenados'
FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_TYPE = 'PROCEDURE'
ORDER BY ROUTINE_NAME;

-- Verificar datos de ejemplo
SELECT COUNT(*) as 'Total Empleados' FROM Empleados;
SELECT COUNT(*) as 'Total Tipos de Pago' FROM TipoPagosAbonos;
SELECT COUNT(*) as 'Total Autorizadores' FROM EmpleadosAutorizadores;
SELECT COUNT(*) as 'Total Préstamos' FROM Prestamos;