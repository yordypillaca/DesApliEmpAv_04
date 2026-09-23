using System.Data;
using Microsoft.Data.SqlClient;
using Neptuno.Data.Models;

namespace Neptuno.Data;

/// <summary>
/// Listados en modo DESCONECTADO (DataSet + SqlDataAdapter.Fill).
/// Altas, ediciones y baja lógica con ExecuteNonQueryAsync.
/// </summary>
public class ProductoRepository
{
    public async Task<List<Producto>> ListarAsync()
    {
        var dataSet = await DisconnectedHelper.FillAsync("usp_Producto_Listar");
        return DisconnectedHelper.Rows(DisconnectedHelper.FirstTable(dataSet)).Select(Mapear).ToList();
    }

    public async Task<int> InsertarAsync(Producto producto)
    {
        await using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        await using var command = new SqlCommand("usp_Producto_Insertar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametros(command, producto, incluirId: false);
        var idParam = command.Parameters.Add("@ProductoID", SqlDbType.Int);
        idParam.Direction = ParameterDirection.Output;

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
        return (int)idParam.Value;
    }

    public async Task ActualizarAsync(Producto producto)
    {
        await using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        await using var command = new SqlCommand("usp_Producto_Actualizar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametros(command, producto, incluirId: true);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task EliminarAsync(int productoId)
    {
        await using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        await using var command = new SqlCommand("usp_Producto_Eliminar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@ProductoID", productoId);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
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

    private static Producto Mapear(DataRow row)
    {
        return new Producto
        {
            ProductoID = row.GetInt32("ProductoID"),
            NombreProducto = row.GetString("NombreProducto"),
            ProveedorID = row.GetNullableInt32("ProveedorID"),
            CategoriaID = row.GetNullableInt32("CategoriaID"),
            CantidadPorUnidad = row.GetNullableString("CantidadPorUnidad"),
            PrecioUnidad = row.GetDecimal("PrecioUnidad"),
            UnidadesEnExistencia = row.GetInt16("UnidadesEnExistencia"),
            UnidadesEnPedido = row.GetInt16("UnidadesEnPedido"),
            NivelDeReorden = row.GetInt16("NivelDeReorden"),
            Descontinuado = row.GetBoolean("Descontinuado"),
            NombreCategoria = row.Table.Columns.Contains("NombreCategoria") ? row.GetNullableString("NombreCategoria") : null,
            NombreProveedor = row.Table.Columns.Contains("NombreProveedor") ? row.GetNullableString("NombreProveedor") : null
        };
    }
}
