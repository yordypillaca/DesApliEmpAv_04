/* ============================================================
   Procedimientos almacenados — NeptunoDB
   ============================================================ */

USE NeptunoDB;
GO

/* ------------------------------------------------------------
   3. CRUD de productos
   ------------------------------------------------------------ */

CREATE OR ALTER PROCEDURE dbo.usp_Producto_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.ProductoID,
        p.NombreProducto,
        p.ProveedorID,
        p.CategoriaID,
        p.CantidadPorUnidad,
        p.PrecioUnidad,
        p.UnidadesEnExistencia,
        p.UnidadesEnPedido,
        p.NivelDeReorden,
        p.Descontinuado,
        c.NombreCategoria,
        pr.CompaniaNombre AS NombreProveedor
    FROM dbo.Productos p
    LEFT JOIN dbo.Categorias c ON c.CategoriaID = p.CategoriaID
    LEFT JOIN dbo.Proveedores pr ON pr.ProveedorID = p.ProveedorID
    WHERE p.Activo = 1
    ORDER BY p.NombreProducto;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Producto_Obtener
    @ProductoID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.ProductoID,
        p.NombreProducto,
        p.ProveedorID,
        p.CategoriaID,
        p.CantidadPorUnidad,
        p.PrecioUnidad,
        p.UnidadesEnExistencia,
        p.UnidadesEnPedido,
        p.NivelDeReorden,
        p.Descontinuado,
        c.NombreCategoria,
        pr.CompaniaNombre AS NombreProveedor
    FROM dbo.Productos p
    LEFT JOIN dbo.Categorias c ON c.CategoriaID = p.CategoriaID
    LEFT JOIN dbo.Proveedores pr ON pr.ProveedorID = p.ProveedorID
    WHERE p.ProductoID = @ProductoID;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Producto_Insertar
    @NombreProducto       NVARCHAR(60),
    @ProveedorID          INT = NULL,
    @CategoriaID          INT = NULL,
    @CantidadPorUnidad    NVARCHAR(30) = NULL,
    @PrecioUnidad         DECIMAL(10,2),
    @UnidadesEnExistencia SMALLINT,
    @UnidadesEnPedido     SMALLINT,
    @NivelDeReorden       SMALLINT,
    @Descontinuado        BIT,
    @ProductoID           INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Productos (
        NombreProducto, ProveedorID, CategoriaID, CantidadPorUnidad,
        PrecioUnidad, UnidadesEnExistencia, UnidadesEnPedido, NivelDeReorden, Descontinuado)
    VALUES (
        @NombreProducto, @ProveedorID, @CategoriaID, @CantidadPorUnidad,
        @PrecioUnidad, @UnidadesEnExistencia, @UnidadesEnPedido, @NivelDeReorden, @Descontinuado);

    SET @ProductoID = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Producto_Actualizar
    @ProductoID           INT,
    @NombreProducto       NVARCHAR(60),
    @ProveedorID          INT = NULL,
    @CategoriaID          INT = NULL,
    @CantidadPorUnidad    NVARCHAR(30) = NULL,
    @PrecioUnidad         DECIMAL(10,2),
    @UnidadesEnExistencia SMALLINT,
    @UnidadesEnPedido     SMALLINT,
    @NivelDeReorden       SMALLINT,
    @Descontinuado        BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Productos
    SET NombreProducto       = @NombreProducto,
        ProveedorID          = @ProveedorID,
        CategoriaID          = @CategoriaID,
        CantidadPorUnidad    = @CantidadPorUnidad,
        PrecioUnidad         = @PrecioUnidad,
        UnidadesEnExistencia = @UnidadesEnExistencia,
        UnidadesEnPedido     = @UnidadesEnPedido,
        NivelDeReorden       = @NivelDeReorden,
        Descontinuado        = @Descontinuado
    WHERE ProductoID = @ProductoID;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Producto_Eliminar
    @ProductoID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Productos
    SET Activo = 0
    WHERE ProductoID = @ProductoID
      AND Activo = 1;
END
GO

/* ------------------------------------------------------------
   4. CRUD de categorías
   ------------------------------------------------------------ */

CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CategoriaID, NombreCategoria, Descripcion
    FROM dbo.Categorias
    WHERE Activo = 1
    ORDER BY NombreCategoria;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Obtener
    @CategoriaID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CategoriaID, NombreCategoria, Descripcion
    FROM dbo.Categorias
    WHERE CategoriaID = @CategoriaID;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Insertar
    @NombreCategoria NVARCHAR(30),
    @Descripcion     NVARCHAR(200) = NULL,
    @CategoriaID     INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Categorias (NombreCategoria, Descripcion)
    VALUES (@NombreCategoria, @Descripcion);

    SET @CategoriaID = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Actualizar
    @CategoriaID     INT,
    @NombreCategoria NVARCHAR(30),
    @Descripcion     NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Categorias
    SET NombreCategoria = @NombreCategoria,
        Descripcion     = @Descripcion
    WHERE CategoriaID = @CategoriaID;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Categoria_Eliminar
    @CategoriaID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Categorias
    SET Activo = 0
    WHERE CategoriaID = @CategoriaID
      AND Activo = 1;
END
GO

/* ------------------------------------------------------------
   5. CRUD de proveedores
   ------------------------------------------------------------ */

CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ProveedorID, CompaniaNombre, NombreContacto, CargoContacto,
           Direccion, Ciudad, CodigoPostal, Pais, Telefono, Fax
    FROM dbo.Proveedores
    WHERE Activo = 1
    ORDER BY CompaniaNombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Obtener
    @ProveedorID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ProveedorID, CompaniaNombre, NombreContacto, CargoContacto,
           Direccion, Ciudad, CodigoPostal, Pais, Telefono, Fax
    FROM dbo.Proveedores
    WHERE ProveedorID = @ProveedorID;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Insertar
    @CompaniaNombre NVARCHAR(60),
    @NombreContacto NVARCHAR(40) = NULL,
    @CargoContacto  NVARCHAR(40) = NULL,
    @Direccion      NVARCHAR(80) = NULL,
    @Ciudad         NVARCHAR(30) = NULL,
    @CodigoPostal   NVARCHAR(10) = NULL,
    @Pais           NVARCHAR(30) = NULL,
    @Telefono       NVARCHAR(24) = NULL,
    @Fax            NVARCHAR(24) = NULL,
    @ProveedorID    INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Proveedores (
        CompaniaNombre, NombreContacto, CargoContacto, Direccion,
        Ciudad, CodigoPostal, Pais, Telefono, Fax)
    VALUES (
        @CompaniaNombre, @NombreContacto, @CargoContacto, @Direccion,
        @Ciudad, @CodigoPostal, @Pais, @Telefono, @Fax);

    SET @ProveedorID = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Actualizar
    @ProveedorID    INT,
    @CompaniaNombre NVARCHAR(60),
    @NombreContacto NVARCHAR(40) = NULL,
    @CargoContacto  NVARCHAR(40) = NULL,
    @Direccion      NVARCHAR(80) = NULL,
    @Ciudad         NVARCHAR(30) = NULL,
    @CodigoPostal   NVARCHAR(10) = NULL,
    @Pais           NVARCHAR(30) = NULL,
    @Telefono       NVARCHAR(24) = NULL,
    @Fax            NVARCHAR(24) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Proveedores
    SET CompaniaNombre = @CompaniaNombre,
        NombreContacto = @NombreContacto,
        CargoContacto  = @CargoContacto,
        Direccion      = @Direccion,
        Ciudad         = @Ciudad,
        CodigoPostal   = @CodigoPostal,
        Pais           = @Pais,
        Telefono       = @Telefono,
        Fax            = @Fax
    WHERE ProveedorID = @ProveedorID;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Eliminar
    @ProveedorID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Proveedores
    SET Activo = 0
    WHERE ProveedorID = @ProveedorID
      AND Activo = 1;
END
GO

/* ------------------------------------------------------------
   7. Listado de proveedores por NombreContacto y Ciudad
   ------------------------------------------------------------ */

CREATE OR ALTER PROCEDURE dbo.usp_Proveedor_Buscar
    @NombreContacto NVARCHAR(40) = NULL,
    @Ciudad         NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ProveedorID, CompaniaNombre, NombreContacto, CargoContacto,
           Direccion, Ciudad, CodigoPostal, Pais, Telefono, Fax
    FROM dbo.Proveedores
    WHERE Activo = 1
      AND (@NombreContacto IS NULL OR @NombreContacto = N'' OR NombreContacto LIKE N'%' + @NombreContacto + N'%')
      AND (@Ciudad IS NULL OR @Ciudad = N'' OR Ciudad LIKE N'%' + @Ciudad + N'%')
    ORDER BY CompaniaNombre;
END
GO

/* ------------------------------------------------------------
   6. CRUD de pedidos (cabecera + detalle)
   ------------------------------------------------------------ */

CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.PedidoID,
        p.ClienteID,
        p.EmpleadoID,
        p.FechaPedido,
        p.FechaRequerida,
        p.FechaEnvio,
        p.TransportistaID,
        p.Destinatario,
        p.CiudadDestino,
        p.PaisDestino,
        c.Empresa AS NombreCliente,
        e.Nombre + N' ' + e.Apellidos AS NombreEmpleado,
        t.CompaniaNombre AS NombreTransportista
    FROM dbo.Pedidos p
    LEFT JOIN dbo.Clientes c ON c.ClienteID = p.ClienteID
    LEFT JOIN dbo.Empleados e ON e.EmpleadoID = p.EmpleadoID
    LEFT JOIN dbo.Transportistas t ON t.TransportistaID = p.TransportistaID
    WHERE p.Activo = 1
    ORDER BY p.FechaPedido DESC, p.PedidoID DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Obtener
    @PedidoID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.PedidoID,
        p.ClienteID,
        p.EmpleadoID,
        p.FechaPedido,
        p.FechaRequerida,
        p.FechaEnvio,
        p.TransportistaID,
        p.Destinatario,
        p.CiudadDestino,
        p.PaisDestino,
        c.Empresa AS NombreCliente,
        e.Nombre + N' ' + e.Apellidos AS NombreEmpleado,
        t.CompaniaNombre AS NombreTransportista
    FROM dbo.Pedidos p
    LEFT JOIN dbo.Clientes c ON c.ClienteID = p.ClienteID
    LEFT JOIN dbo.Empleados e ON e.EmpleadoID = p.EmpleadoID
    LEFT JOIN dbo.Transportistas t ON t.TransportistaID = p.TransportistaID
    WHERE p.PedidoID = @PedidoID;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Insertar
    @ClienteID       INT = NULL,
    @EmpleadoID      INT = NULL,
    @FechaPedido     DATE,
    @FechaRequerida  DATE = NULL,
    @FechaEnvio      DATE = NULL,
    @TransportistaID INT = NULL,
    @Destinatario    NVARCHAR(60) = NULL,
    @CiudadDestino   NVARCHAR(30) = NULL,
    @PaisDestino     NVARCHAR(30) = NULL,
    @PedidoID        INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Pedidos (
        ClienteID, EmpleadoID, FechaPedido, FechaRequerida, FechaEnvio,
        TransportistaID, Destinatario, CiudadDestino, PaisDestino)
    VALUES (
        @ClienteID, @EmpleadoID, @FechaPedido, @FechaRequerida, @FechaEnvio,
        @TransportistaID, @Destinatario, @CiudadDestino, @PaisDestino);

    SET @PedidoID = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Actualizar
    @PedidoID        INT,
    @ClienteID       INT = NULL,
    @EmpleadoID      INT = NULL,
    @FechaPedido     DATE,
    @FechaRequerida  DATE = NULL,
    @FechaEnvio      DATE = NULL,
    @TransportistaID INT = NULL,
    @Destinatario    NVARCHAR(60) = NULL,
    @CiudadDestino   NVARCHAR(30) = NULL,
    @PaisDestino     NVARCHAR(30) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Pedidos
    SET ClienteID       = @ClienteID,
        EmpleadoID      = @EmpleadoID,
        FechaPedido     = @FechaPedido,
        FechaRequerida  = @FechaRequerida,
        FechaEnvio      = @FechaEnvio,
        TransportistaID = @TransportistaID,
        Destinatario    = @Destinatario,
        CiudadDestino   = @CiudadDestino,
        PaisDestino     = @PaisDestino
    WHERE PedidoID = @PedidoID;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Pedido_Eliminar
    @PedidoID INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Pedidos
    SET Activo = 0
    WHERE PedidoID = @PedidoID
      AND Activo = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_DetallePedido_ListarPorPedido
    @PedidoID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        d.PedidoID,
        d.ProductoID,
        d.PrecioUnidad,
        d.Cantidad,
        d.Descuento,
        pr.NombreProducto,
        CAST(d.PrecioUnidad * d.Cantidad * (1 - d.Descuento) AS DECIMAL(12,2)) AS Importe
    FROM dbo.DetallePedidos d
    INNER JOIN dbo.Productos pr ON pr.ProductoID = d.ProductoID
    WHERE d.PedidoID = @PedidoID
    ORDER BY pr.NombreProducto;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_DetallePedido_Insertar
    @PedidoID     INT,
    @ProductoID   INT,
    @PrecioUnidad DECIMAL(10,2),
    @Cantidad     SMALLINT,
    @Descuento    DECIMAL(4,2)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.DetallePedidos (PedidoID, ProductoID, PrecioUnidad, Cantidad, Descuento)
    VALUES (@PedidoID, @ProductoID, @PrecioUnidad, @Cantidad, @Descuento);
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_DetallePedido_EliminarPorPedido
    @PedidoID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.DetallePedidos WHERE PedidoID = @PedidoID;
END
GO

/* ------------------------------------------------------------
   8. Detalles de pedidos con INNER JOIN, filtro por fechas
   ------------------------------------------------------------ */

CREATE OR ALTER PROCEDURE dbo.usp_DetallePedido_ListarPorFechas
    @FechaInicio DATE,
    @FechaFin    DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        d.PedidoID,
        p.FechaPedido,
        p.Destinatario,
        p.CiudadDestino,
        d.ProductoID,
        pr.NombreProducto,
        d.PrecioUnidad,
        d.Cantidad,
        d.Descuento,
        CAST(d.PrecioUnidad * d.Cantidad * (1 - d.Descuento) AS DECIMAL(12,2)) AS Importe
    FROM dbo.DetallePedidos d
    INNER JOIN dbo.Pedidos p ON p.PedidoID = d.PedidoID
    INNER JOIN dbo.Productos pr ON pr.ProductoID = d.ProductoID
    WHERE p.FechaPedido BETWEEN @FechaInicio AND @FechaFin
      AND p.Activo = 1
    ORDER BY p.FechaPedido, d.PedidoID, pr.NombreProducto;
END
GO

/* Catálogos auxiliares para combos del mantenimiento de pedidos */

CREATE OR ALTER PROCEDURE dbo.usp_Cliente_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ClienteID, Empresa, NombreContacto, Ciudad, Pais, Telefono
    FROM dbo.Clientes
    ORDER BY Empresa;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Empleado_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT EmpleadoID, Nombre, Apellidos, Cargo, Ciudad, Pais,
           Nombre + N' ' + Apellidos AS NombreCompleto
    FROM dbo.Empleados
    ORDER BY Apellidos, Nombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Transportista_Listar
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TransportistaID, CompaniaNombre, Telefono
    FROM dbo.Transportistas
    ORDER BY CompaniaNombre;
END
GO
