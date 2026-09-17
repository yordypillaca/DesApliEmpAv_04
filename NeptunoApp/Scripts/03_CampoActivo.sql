/* ============================================================
   Campo Activo para eliminación lógica
   Tablas: Productos, Categorias, Proveedores, Pedidos
   ============================================================ */

USE NeptunoDB;
GO

IF COL_LENGTH(N'dbo.Categorias', N'Activo') IS NULL
BEGIN
    ALTER TABLE dbo.Categorias
    ADD Activo BIT NOT NULL CONSTRAINT DF_Categorias_Activo DEFAULT (1);
END
GO

IF COL_LENGTH(N'dbo.Proveedores', N'Activo') IS NULL
BEGIN
    ALTER TABLE dbo.Proveedores
    ADD Activo BIT NOT NULL CONSTRAINT DF_Proveedores_Activo DEFAULT (1);
END
GO

IF COL_LENGTH(N'dbo.Productos', N'Activo') IS NULL
BEGIN
    ALTER TABLE dbo.Productos
    ADD Activo BIT NOT NULL CONSTRAINT DF_Productos_Activo DEFAULT (1);
END
GO

IF COL_LENGTH(N'dbo.Pedidos', N'Activo') IS NULL
BEGIN
    ALTER TABLE dbo.Pedidos
    ADD Activo BIT NOT NULL CONSTRAINT DF_Pedidos_Activo DEFAULT (1);
END
GO
