-- Script para remover a coluna ClienteId da tabela ApartamentosReservados
-- Execute este script na base de dados ghotel

USE ghotel;
GO

-- Verificar se a coluna existe antes de tentar removê-la
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'ApartamentosReservados' 
           AND COLUMN_NAME = 'ClienteId')
BEGIN
    -- Remover foreign key constraint se existir
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS 
               WHERE CONSTRAINT_NAME LIKE '%ClienteId%' 
               AND CONSTRAINT_SCHEMA = 'dbo')
    BEGIN
        DECLARE @ConstraintName NVARCHAR(200)
        SELECT @ConstraintName = CONSTRAINT_NAME 
        FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS 
        WHERE CONSTRAINT_NAME LIKE '%ClienteId%' 
        AND CONSTRAINT_SCHEMA = 'dbo'
        
        EXEC('ALTER TABLE ApartamentosReservados DROP CONSTRAINT ' + @ConstraintName)
        PRINT 'Foreign key constraint removed: ' + @ConstraintName
    END
    
    -- Remover índice se existir
    IF EXISTS (SELECT * FROM sys.indexes 
               WHERE name LIKE '%ClienteId%' 
               AND object_id = OBJECT_ID('ApartamentosReservados'))
    BEGIN
        DROP INDEX IX_ApartamentosReservados_ClienteId ON ApartamentosReservados
        PRINT 'Index IX_ApartamentosReservados_ClienteId removed'
    END
    
    -- Remover a coluna
    ALTER TABLE ApartamentosReservados DROP COLUMN ClienteId
    PRINT 'Column ClienteId removed from ApartamentosReservados table'
END
ELSE
BEGIN
    PRINT 'Column ClienteId does not exist in ApartamentosReservados table'
END
GO
