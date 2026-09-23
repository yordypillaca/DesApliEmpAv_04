using System.Data;
using Microsoft.Data.SqlClient;
using Neptuno.Data.Models;

namespace Neptuno.Data;

/// <summary>
/// Consultas de pedidos/reportes en modo DESCONECTADO (DataSet).
/// Guardado y baja lógica con ExecuteNonQueryAsync.
/// </summary>
public class PedidoRepository
{
    public async Task<List<Pedido>> ListarAsync()
    {
        var dataSet = await DisconnectedHelper.FillAsync("usp_Pedido_Listar");
        return DisconnectedHelper.Rows(DisconnectedHelper.FirstTable(dataSet)).Select(MapearPedido).ToList();
    }

    public async Task<List<DetallePedido>> ListarDetallesAsync(int pedidoId)
    {
        var dataSet = await DisconnectedHelper.FillAsync("usp_DetallePedido_ListarPorPedido",
            command => command.Parameters.AddWithValue("@PedidoID", pedidoId));
        return DisconnectedHelper.Rows(DisconnectedHelper.FirstTable(dataSet)).Select(MapearDetalle).ToList();
    }

    public async Task<List<DetallePedido>> ListarDetallesPorFechasAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        var dataSet = await DisconnectedHelper.FillAsync("usp_DetallePedido_ListarPorFechas", command =>
        {
            command.Parameters.AddWithValue("@FechaInicio", fechaInicio.Date);
            command.Parameters.AddWithValue("@FechaFin", fechaFin.Date);
        });
        return DisconnectedHelper.Rows(DisconnectedHelper.FirstTable(dataSet)).Select(MapearDetalleReporte).ToList();
    }

    public async Task<int> GuardarAsync(Pedido pedido, IReadOnlyList<DetallePedido> detalles)
    {
        await using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        await connection.OpenAsync();
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

        try
        {
            int pedidoId = pedido.PedidoID > 0
                ? await ActualizarCabeceraAsync(connection, transaction, pedido)
                : await InsertarCabeceraAsync(connection, transaction, pedido);

            await using (var borrar = new SqlCommand("usp_DetallePedido_EliminarPorPedido", connection, transaction))
            {
                borrar.CommandType = CommandType.StoredProcedure;
                borrar.Parameters.AddWithValue("@PedidoID", pedidoId);
                await borrar.ExecuteNonQueryAsync();
            }

            foreach (var linea in detalles)
            {
                await using var insertar = new SqlCommand("usp_DetallePedido_Insertar", connection, transaction);
                insertar.CommandType = CommandType.StoredProcedure;
                insertar.Parameters.AddWithValue("@PedidoID", pedidoId);
                insertar.Parameters.AddWithValue("@ProductoID", linea.ProductoID);
                insertar.Parameters.AddWithValue("@PrecioUnidad", linea.PrecioUnidad);
                insertar.Parameters.AddWithValue("@Cantidad", linea.Cantidad);
                insertar.Parameters.AddWithValue("@Descuento", linea.Descuento);
                await insertar.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
            return pedidoId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task EliminarAsync(int pedidoId)
    {
        await using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        await using var command = new SqlCommand("usp_Pedido_Eliminar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@PedidoID", pedidoId);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    private static async Task<int> InsertarCabeceraAsync(SqlConnection connection, SqlTransaction transaction, Pedido pedido)
    {
        await using var command = new SqlCommand("usp_Pedido_Insertar", connection, transaction)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametrosCabecera(command, pedido, incluirId: false);
        var idParam = command.Parameters.Add("@PedidoID", SqlDbType.Int);
        idParam.Direction = ParameterDirection.Output;
        await command.ExecuteNonQueryAsync();
        return (int)idParam.Value;
    }

    private static async Task<int> ActualizarCabeceraAsync(SqlConnection connection, SqlTransaction transaction, Pedido pedido)
    {
        await using var command = new SqlCommand("usp_Pedido_Actualizar", connection, transaction)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametrosCabecera(command, pedido, incluirId: true);
        await command.ExecuteNonQueryAsync();
        return pedido.PedidoID;
    }

    private static void AgregarParametrosCabecera(SqlCommand command, Pedido pedido, bool incluirId)
    {
        if (incluirId)
        {
            command.Parameters.AddWithValue("@PedidoID", pedido.PedidoID);
        }

        command.Parameters.AddWithValue("@ClienteID", (object?)pedido.ClienteID ?? DBNull.Value);
        command.Parameters.AddWithValue("@EmpleadoID", (object?)pedido.EmpleadoID ?? DBNull.Value);
        command.Parameters.AddWithValue("@FechaPedido", pedido.FechaPedido.Date);
        command.Parameters.AddWithValue("@FechaRequerida", (object?)pedido.FechaRequerida?.Date ?? DBNull.Value);
        command.Parameters.AddWithValue("@FechaEnvio", (object?)pedido.FechaEnvio?.Date ?? DBNull.Value);
        command.Parameters.AddWithValue("@TransportistaID", (object?)pedido.TransportistaID ?? DBNull.Value);
        command.Parameters.AddWithValue("@Destinatario", (object?)pedido.Destinatario ?? DBNull.Value);
        command.Parameters.AddWithValue("@CiudadDestino", (object?)pedido.CiudadDestino ?? DBNull.Value);
        command.Parameters.AddWithValue("@PaisDestino", (object?)pedido.PaisDestino ?? DBNull.Value);
    }

    private static Pedido MapearPedido(DataRow row)
    {
        return new Pedido
        {
            PedidoID = row.GetInt32("PedidoID"),
            ClienteID = row.GetNullableInt32("ClienteID"),
            EmpleadoID = row.GetNullableInt32("EmpleadoID"),
            FechaPedido = row.GetDateTime("FechaPedido"),
            FechaRequerida = row.GetNullableDateTime("FechaRequerida"),
            FechaEnvio = row.GetNullableDateTime("FechaEnvio"),
            TransportistaID = row.GetNullableInt32("TransportistaID"),
            Destinatario = row.GetNullableString("Destinatario"),
            CiudadDestino = row.GetNullableString("CiudadDestino"),
            PaisDestino = row.GetNullableString("PaisDestino"),
            NombreCliente = row.GetNullableString("NombreCliente"),
            NombreEmpleado = row.GetNullableString("NombreEmpleado"),
            NombreTransportista = row.GetNullableString("NombreTransportista")
        };
    }

    private static DetallePedido MapearDetalle(DataRow row)
    {
        return new DetallePedido
        {
            PedidoID = row.GetInt32("PedidoID"),
            ProductoID = row.GetInt32("ProductoID"),
            PrecioUnidad = row.GetDecimal("PrecioUnidad"),
            Cantidad = row.GetInt16("Cantidad"),
            Descuento = row.GetDecimal("Descuento"),
            NombreProducto = row.GetNullableString("NombreProducto"),
            Importe = row.GetDecimal("Importe")
        };
    }

    private static DetallePedido MapearDetalleReporte(DataRow row)
    {
        return new DetallePedido
        {
            PedidoID = row.GetInt32("PedidoID"),
            FechaPedido = row.GetNullableDateTime("FechaPedido"),
            Destinatario = row.GetNullableString("Destinatario"),
            CiudadDestino = row.GetNullableString("CiudadDestino"),
            ProductoID = row.GetInt32("ProductoID"),
            NombreProducto = row.GetNullableString("NombreProducto"),
            PrecioUnidad = row.GetDecimal("PrecioUnidad"),
            Cantidad = row.GetInt16("Cantidad"),
            Descuento = row.GetDecimal("Descuento"),
            Importe = row.GetDecimal("Importe")
        };
    }
}
