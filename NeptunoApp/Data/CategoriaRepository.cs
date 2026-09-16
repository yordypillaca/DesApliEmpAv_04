using System.Data;
using Microsoft.Data.SqlClient;
using NeptunoApp.Models;

namespace NeptunoApp.Data;

/// <summary>
/// Acceso a categorías en modo CONECTADO mediante procedimientos almacenados.
/// </summary>
public class CategoriaRepository
{
    public List<Categoria> Listar()
    {
        var lista = new List<Categoria>();
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_Categoria_Listar", connection)
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

    public int Insertar(Categoria categoria)
    {
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_Categoria_Insertar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@NombreCategoria", categoria.NombreCategoria);
        command.Parameters.AddWithValue("@Descripcion", (object?)categoria.Descripcion ?? DBNull.Value);
        var idParam = command.Parameters.Add("@CategoriaID", SqlDbType.Int);
        idParam.Direction = ParameterDirection.Output;

        connection.Open();
        command.ExecuteNonQuery();
        return (int)idParam.Value;
    }

    public void Actualizar(Categoria categoria)
    {
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_Categoria_Actualizar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@CategoriaID", categoria.CategoriaID);
        command.Parameters.AddWithValue("@NombreCategoria", categoria.NombreCategoria);
        command.Parameters.AddWithValue("@Descripcion", (object?)categoria.Descripcion ?? DBNull.Value);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Eliminar(int categoriaId)
    {
        using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        using var command = new SqlCommand("usp_Categoria_Eliminar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@CategoriaID", categoriaId);

        connection.Open();
        command.ExecuteNonQuery();
    }

    private static Categoria Leer(SqlDataReader reader)
    {
        return new Categoria
        {
            CategoriaID = reader.GetInt32(0),
            NombreCategoria = reader.GetString(1),
            Descripcion = reader.GetNullableString(2)
        };
    }
}
