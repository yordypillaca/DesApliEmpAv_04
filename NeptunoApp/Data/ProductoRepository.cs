using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data;

/// <summary>
/// Acceso a productos en modo CONECTADO.
/// Las escrituras (insertar, actualizar y baja lógica) usan ExecuteNonQuery.
/// </summary>
public class ProductoRepository
{
    public List<Producto> Listar()
    {
        var lista = new List<Producto>();
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_Producto_Listar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(Leer(reader));
        }

        return lista;
    }

    public Producto? Obtener(int productoId)
    {
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_Producto_Obtener", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@ProductoID", productoId);

        connection.Open();
        using var reader = command.ExecuteReader();
        return reader.Read() ? Leer(reader) : null;
    }

    public int Insertar(Producto producto)
    {
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_Producto_Insertar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametros(command, producto, incluirId: false);
        var idParam = command.Parameters.Add("@ProductoID", SqlDbType.Int);
        idParam.Direction = ParameterDirection.Output;

        connection.Open();
        // Alta: ExecuteNonQuery ejecuta usp_Producto_Insertar.
        command.ExecuteNonQuery();
        return (int)idParam.Value;
    }

    public void Actualizar(Producto producto)
    {
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_Producto_Actualizar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametros(command, producto, incluirId: true);

        connection.Open();
        // Edición: ExecuteNonQuery ejecuta usp_Producto_Actualizar.
        command.ExecuteNonQuery();
    }

    public void Eliminar(int productoId)
    {
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_Producto_Eliminar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@ProductoID", productoId);

        connection.Open();
        // Baja lógica: ExecuteNonQuery ejecuta usp_Producto_Eliminar (Activo = 0).
        command.ExecuteNonQuery();
    }

    private static void AgregarParametros(SqlCommand command, Producto producto, bool incluirId)
    {
        if (incluirId)
        {
            command.Parameters.AddWithValue("@ProductoID", producto.ProductoID);
        }

        command.Parameters.AddWithValue("@NombreProducto", producto.NombreProducto);
        command.Parameters.AddWithValue("@ProveedorID", (object?)producto.ProveedorID ?? DBNull.Value);
        command.Parameters.AddWithValue("@CategoriaID", (object?)producto.CategoriaID ?? DBNull.Value);
        command.Parameters.AddWithValue("@CantidadPorUnidad", (object?)producto.CantidadPorUnidad ?? DBNull.Value);
        command.Parameters.AddWithValue("@PrecioUnidad", producto.PrecioUnidad);
        command.Parameters.AddWithValue("@UnidadesEnExistencia", producto.UnidadesEnExistencia);
        command.Parameters.AddWithValue("@UnidadesEnPedido", producto.UnidadesEnPedido);
        command.Parameters.AddWithValue("@NivelDeReorden", producto.NivelDeReorden);
        command.Parameters.AddWithValue("@Descontinuado", producto.Descontinuado);
    }

    private static Producto Leer(SqlDataReader reader)
    {
        return new Producto
        {
            ProductoID = reader.GetInt32(0),
            NombreProducto = reader.GetString(1),
            ProveedorID = reader.GetNullableInt32(2),
            CategoriaID = reader.GetNullableInt32(3),
            CantidadPorUnidad = reader.GetNullableString(4),
            PrecioUnidad = reader.GetDecimal(5),
            UnidadesEnExistencia = reader.GetInt16Safe(6),
            UnidadesEnPedido = reader.GetInt16Safe(7),
            NivelDeReorden = reader.GetInt16Safe(8),
            Descontinuado = reader.GetBoolean(9),
            NombreCategoria = reader.GetNullableString(10),
            NombreProveedor = reader.GetNullableString(11)
        };
    }
}
