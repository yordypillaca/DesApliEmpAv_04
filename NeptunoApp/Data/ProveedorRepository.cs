using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data;

/// <summary>
/// Acceso a proveedores en modo CONECTADO.
/// Las escrituras (insertar, actualizar y baja lógica) usan ExecuteNonQuery.
/// El listado y la búsqueda solo devuelven registros con Activo = 1.
/// </summary>
public class ProveedorRepository
{
    public List<Proveedor> Listar()
    {
        var lista = new List<Proveedor>();
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_Proveedor_Listar", connection)
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

    public List<Proveedor> Buscar(string? nombreContacto, string? ciudad)
    {
        var lista = new List<Proveedor>();
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_Proveedor_Buscar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@NombreContacto", (object?)nombreContacto ?? DBNull.Value);
        command.Parameters.AddWithValue("@Ciudad", (object?)ciudad ?? DBNull.Value);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(Leer(reader));
        }

        return lista;
    }

    public int Insertar(Proveedor proveedor)
    {
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_Proveedor_Insertar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametros(command, proveedor, incluirId: false);
        var idParam = command.Parameters.Add("@ProveedorID", SqlDbType.Int);
        idParam.Direction = ParameterDirection.Output;

        connection.Open();
        // Alta: ExecuteNonQuery ejecuta usp_Proveedor_Insertar.
        command.ExecuteNonQuery();
        return (int)idParam.Value;
    }

    public void Actualizar(Proveedor proveedor)
    {
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_Proveedor_Actualizar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarParametros(command, proveedor, incluirId: true);

        connection.Open();
        // Edición: ExecuteNonQuery ejecuta usp_Proveedor_Actualizar.
        command.ExecuteNonQuery();
    }

    public void Eliminar(int proveedorId)
    {
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_Proveedor_Eliminar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@ProveedorID", proveedorId);

        connection.Open();
        // Baja lógica: ExecuteNonQuery ejecuta usp_Proveedor_Eliminar (Activo = 0).
        command.ExecuteNonQuery();
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

    private static Proveedor Leer(SqlDataReader reader)
    {
        return new Proveedor
        {
            ProveedorID = reader.GetInt32(0),
            CompaniaNombre = reader.GetString(1),
            NombreContacto = reader.GetNullableString(2),
            CargoContacto = reader.GetNullableString(3),
            Direccion = reader.GetNullableString(4),
            Ciudad = reader.GetNullableString(5),
            CodigoPostal = reader.GetNullableString(6),
            Pais = reader.GetNullableString(7),
            Telefono = reader.GetNullableString(8),
            Fax = reader.GetNullableString(9)
        };
    }
}
