-- Script para remover a coluna ClienteId da tabela ApartamentosReservados
-- Execute este script no banco de dados SQL Server

USE [ghotel];
GO

-- Primeiro, verificar se a coluna existe
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'ApartamentosReservados' 
    AND COLUMN_NAME = 'ClienteId'
)
BEGIN
    PRINT 'Removendo a coluna ClienteId da tabela ApartamentosReservados...'
    
    -- Primeiro, remover qualquer constraint de foreign key associada à coluna ClienteId
    DECLARE @ConstraintName NVARCHAR(200)
    SELECT @ConstraintName = name
    FROM sys.foreign_keys
    WHERE parent_object_id = OBJECT_ID('ApartamentosReservados')
    AND referenced_object_id = OBJECT_ID('Clientes')
    AND EXISTS (
        SELECT 1 FROM sys.foreign_key_columns fkc
        INNER JOIN sys.columns c ON fkc.parent_column_id = c.column_id AND fkc.parent_object_id = c.object_id
        WHERE fkc.constraint_object_id = sys.foreign_keys.object_id
        AND c.name = 'ClienteId'
    )

    IF @ConstraintName IS NOT NULL
    BEGIN
        EXEC('ALTER TABLE ApartamentosReservados DROP CONSTRAINT ' + @ConstraintName)
        PRINT 'Constraint de foreign key removida: ' + @ConstraintName
    END

    -- Remover qualquer índice associado à coluna ClienteId
    DECLARE @IndexName NVARCHAR(200)
    SELECT @IndexName = i.name
    FROM sys.indexes i
    INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
    INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
    WHERE i.object_id = OBJECT_ID('ApartamentosReservados')
    AND c.name = 'ClienteId'
    AND i.is_primary_key = 0

    IF @IndexName IS NOT NULL
    BEGIN
        EXEC('DROP INDEX ' + @IndexName + ' ON ApartamentosReservados')
        PRINT 'Índice removido: ' + @IndexName
    END

    -- Finalmente, remover a coluna
    ALTER TABLE ApartamentosReservados DROP COLUMN ClienteId
    PRINT 'Coluna ClienteId removida com sucesso!'
END
ELSE
BEGIN
    PRINT 'A coluna ClienteId não existe na tabela ApartamentosReservados.'
END
GO
