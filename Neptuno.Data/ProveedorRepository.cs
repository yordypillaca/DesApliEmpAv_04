using System.Data;
using Microsoft.Data.SqlClient;
using Neptuno.Data.Models;

namespace Neptuno.Data;

/// <summary>
/// Listados y búsqueda en modo DESCONECTADO (DataSet).
/// Escrituras con ExecuteNonQueryAsync.
/// </summary>
public class ProveedorRepository
{
    public async Task<List<Proveedor>> ListarAsync()
    {
        var dataSet = await DisconnectedHelper.FillAsync("usp_Proveedor_Listar");
        return MapearTabla(DisconnectedHelper.FirstTable(dataSet));
    }

    public async Task<List<Proveedor>> BuscarAsync(string? nombreContacto, string? ciudad)
    {
        var dataSet = await DisconnectedHelper.FillAsync("usp_Proveedor_Buscar", command =>
        {
            command.Parameters.AddWithValue("@NombreContacto", (object?)nombreContacto ?? DBNull.Value);
            command.Parameters.AddWithValue("@Ciudad", (object?)ciudad ?? DBNull.Value);
        });
        return MapearTabla(DisconnectedHelper.FirstTable(dataSet));
    }

    public async Task<int> InsertarAsync(Proveedor proveedor)
    {
        await using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        await using var command = new SqlCommand("usp_Proveedor_Insertar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametros(command, proveedor, incluirId: false);
        var idParam = command.Parameters.Add("@ProveedorID", SqlDbType.Int);
        idParam.Direction = ParameterDirection.Output;

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
        return (int)idParam.Value;
    }

    public async Task ActualizarAsync(Proveedor proveedor)
    {
        await using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        await using var command = new SqlCommand("usp_Proveedor_Actualizar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametros(command, proveedor, incluirId: true);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task EliminarAsync(int proveedorId)
    {
        await using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        await using var command = new SqlCommand("usp_Proveedor_Eliminar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@ProveedorID", proveedorId);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    private static void AgregarParametros(SqlCommand command, Proveedor proveedor, bool incluirId)
    {
        if (incluirId)
        {
            command.Parameters.AddWithValue("@ProveedorID", proveedor.ProveedorID);
        }

        command.Parameters.AddWithValue("@CompaniaNombre", proveedor.CompaniaNombre);
        command.Parameters.AddWithValue("@NombreContacto", (object?)proveedor.NombreContacto ?? DBNull.Value);
        command.Parameters.AddWithValue("@CargoContacto", (object?)proveedor.CargoContacto ?? DBNull.Value);
        command.Parameters.AddWithValue("@Direccion", (object?)proveedor.Direccion ?? DBNull.Value);
        command.Parameters.AddWithValue("@Ciudad", (object?)proveedor.Ciudad ?? DBNull.Value);
        command.Parameters.AddWithValue("@CodigoPostal", (object?)proveedor.CodigoPostal ?? DBNull.Value);
        command.Parameters.AddWithValue("@Pais", (object?)proveedor.Pais ?? DBNull.Value);
        command.Parameters.AddWithValue("@Telefono", (object?)proveedor.Telefono ?? DBNull.Value);
        command.Parameters.AddWithValue("@Fax", (object?)proveedor.Fax ?? DBNull.Value);
    }

    private static List<Proveedor> MapearTabla(DataTable tabla)
        => DisconnectedHelper.Rows(tabla).Select(Mapear).ToList();

    private static Proveedor Mapear(DataRow row)
    {
        return new Proveedor
        {
            ProveedorID = row.GetInt32("ProveedorID"),
            CompaniaNombre = row.GetString("CompaniaNombre"),
            NombreContacto = row.GetNullableString("NombreContacto"),
            CargoContacto = row.GetNullableString("CargoContacto"),
            Direccion = row.GetNullableString("Direccion"),
            Ciudad = row.GetNullableString("Ciudad"),
            CodigoPostal = row.GetNullableString("CodigoPostal"),
            Pais = row.GetNullableString("Pais"),
            Telefono = row.GetNullableString("Telefono"),
            Fax = row.GetNullableString("Fax")
        };
    }
}
