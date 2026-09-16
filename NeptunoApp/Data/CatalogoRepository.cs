using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data;

/// <summary>
/// Catálogos auxiliares (clientes, empleados, transportistas) para combos.
/// </summary>
public class CatalogoRepository
{
    public List<Cliente> ListarClientes()
    {
        var lista = new List<Cliente>();
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_Cliente_Listar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new Cliente
            {
                ClienteID = reader.GetInt32(0),
                Empresa = reader.GetString(1),
                NombreContacto = reader.GetNullableString(2),
                Ciudad = reader.GetNullableString(3),
                Pais = reader.GetNullableString(4),
                Telefono = reader.GetNullableString(5)
            });
        }

        return lista;
    }

    public List<Empleado> ListarEmpleados()
    {
        var lista = new List<Empleado>();
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_Empleado_Listar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new Empleado
            {
                EmpleadoID = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Apellidos = reader.GetString(2),
                Cargo = reader.GetNullableString(3),
                Ciudad = reader.GetNullableString(4),
                Pais = reader.GetNullableString(5),
                NombreCompleto = reader.GetString(6)
            });
        }

        return lista;
    }

    public List<Transportista> ListarTransportistas()
    {
        var lista = new List<Transportista>();
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_Transportista_Listar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new Transportista
            {
                TransportistaID = reader.GetInt32(0),
                CompaniaNombre = reader.GetString(1),
                Telefono = reader.GetNullableString(2)
            });
        }

        return lista;
    }
}
