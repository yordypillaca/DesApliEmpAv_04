using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data;

/// <summary>
/// Acceso a pedidos y detalles en modo CONECTADO.
/// Alta, edición y baja lógica de la cabecera se ejecutan con ExecuteNonQuery.
/// </summary>
public class PedidoRepository
{
    public List<Pedido> Listar()
    {
        var lista = new List<Pedido>();
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_Pedido_Listar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(LeerPedido(reader));
        }

        return lista;
    }

    public List<DetallePedido> ListarDetalles(int pedidoId)
    {
        var lista = new List<DetallePedido>();
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_DetallePedido_ListarPorPedido", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@PedidoID", pedidoId);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(LeerDetalle(reader));
        }

        return lista;
    }

    public List<DetallePedido> ListarDetallesPorFechas(DateTime fechaInicio, DateTime fechaFin)
    {
        var lista = new List<DetallePedido>();
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_DetallePedido_ListarPorFechas", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@FechaInicio", fechaInicio.Date);
        command.Parameters.AddWithValue("@FechaFin", fechaFin.Date);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(LeerDetalleReporte(reader));
        }

        return lista;
    }

    public int Guardar(Pedido pedido, IReadOnlyList<DetallePedido> detalles)
    {
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            int pedidoId = pedido.PedidoID > 0
                ? ActualizarCabecera(connection, transaction, pedido)
                : InsertarCabecera(connection, transaction, pedido);

            using (var borrar = new SqlCommand("usp_DetallePedido_EliminarPorPedido", connection, transaction))
            {
                borrar.CommandType = CommandType.StoredProcedure;
                borrar.Parameters.AddWithValue("@PedidoID", pedidoId);
                borrar.ExecuteNonQuery();
            }

            foreach (var linea in detalles)
            {
                using var insertar = new SqlCommand("usp_DetallePedido_Insertar", connection, transaction);
                insertar.CommandType = CommandType.StoredProcedure;
                insertar.Parameters.AddWithValue("@PedidoID", pedidoId);
                insertar.Parameters.AddWithValue("@ProductoID", linea.ProductoID);
                insertar.Parameters.AddWithValue("@PrecioUnidad", linea.PrecioUnidad);
                insertar.Parameters.AddWithValue("@Cantidad", linea.Cantidad);
                insertar.Parameters.AddWithValue("@Descuento", linea.Descuento);
                insertar.ExecuteNonQuery();
            }

            transaction.Commit();
            return pedidoId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void Eliminar(int pedidoId)
    {
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_Pedido_Eliminar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@PedidoID", pedidoId);

        connection.Open();
        // Baja lógica: ExecuteNonQuery ejecuta usp_Pedido_Eliminar (Activo = 0).
        command.ExecuteNonQuery();
    }

    private static int InsertarCabecera(SqlConnection connection, SqlTransaction transaction, Pedido pedido)
    {
        using var command = new SqlCommand("usp_Pedido_Insertar", connection, transaction)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametrosCabecera(command, pedido, incluirId: false);
        var idParam = command.Parameters.Add("@PedidoID", SqlDbType.Int);
        idParam.Direction = ParameterDirection.Output;
        // Alta: ExecuteNonQuery ejecuta usp_Pedido_Insertar.
        command.ExecuteNonQuery();
        return (int)idParam.Value;
    }

    private static int ActualizarCabecera(SqlConnection connection, SqlTransaction transaction, Pedido pedido)
    {
        using var command = new SqlCommand("usp_Pedido_Actualizar", connection, transaction)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametrosCabecera(command, pedido, incluirId: true);
        // Edición: ExecuteNonQuery ejecuta usp_Pedido_Actualizar.
        command.ExecuteNonQuery();
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

    private static Pedido LeerPedido(SqlDataReader reader)
    {
        return new Pedido
        {
            PedidoID = reader.GetInt32(0),
            ClienteID = reader.GetNullableInt32(1),
            EmpleadoID = reader.GetNullableInt32(2),
            FechaPedido = reader.GetDateTime(3),
            FechaRequerida = reader.GetNullableDateTime(4),
            FechaEnvio = reader.GetNullableDateTime(5),
            TransportistaID = reader.GetNullableInt32(6),
            Destinatario = reader.GetNullableString(7),
            CiudadDestino = reader.GetNullableString(8),
            PaisDestino = reader.GetNullableString(9),
            NombreCliente = reader.GetNullableString(10),
            NombreEmpleado = reader.GetNullableString(11),
            NombreTransportista = reader.GetNullableString(12)
        };
    }

    private static DetallePedido LeerDetalle(SqlDataReader reader)
    {
        return new DetallePedido
        {
            PedidoID = reader.GetInt32(0),
            ProductoID = reader.GetInt32(1),
            PrecioUnidad = reader.GetDecimal(2),
            Cantidad = reader.GetInt16Safe(3),
            Descuento = reader.GetDecimal(4),
            NombreProducto = reader.GetNullableString(5),
            Importe = reader.GetDecimal(6)
        };
    }

    private static DetallePedido LeerDetalleReporte(SqlDataReader reader)
    {
        return new DetallePedido
        {
            PedidoID = reader.GetInt32(0),
            FechaPedido = reader.GetDateTime(1),
            Destinatario = reader.GetNullableString(2),
            CiudadDestino = reader.GetNullableString(3),
            ProductoID = reader.GetInt32(4),
            NombreProducto = reader.GetNullableString(5),
            PrecioUnidad = reader.GetDecimal(6),
            Cantidad = reader.GetInt16Safe(7),
            Descuento = reader.GetDecimal(8),
            Importe = reader.GetDecimal(9)
        };
    }
}
