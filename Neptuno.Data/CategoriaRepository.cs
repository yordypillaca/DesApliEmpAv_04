using System.Data;
using Microsoft.Data.SqlClient;
using Neptuno.Data.Models;

namespace Neptuno.Data;

/// <summary>
/// Listados en modo DESCONECTADO (DataSet). Escrituras con ExecuteNonQueryAsync.
/// </summary>
public class CategoriaRepository
{
    public async Task<List<Categoria>> ListarAsync()
    {
        var dataSet = await DisconnectedHelper.FillAsync("usp_Categoria_Listar");
        return DisconnectedHelper.Rows(DisconnectedHelper.FirstTable(dataSet)).Select(Mapear).ToList();
    }

    public async Task<int> InsertarAsync(Categoria categoria)
    {
        await using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        await using var command = new SqlCommand("usp_Categoria_Insertar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@NombreCategoria", categoria.NombreCategoria);
        command.Parameters.AddWithValue("@Descripcion", (object?)categoria.Descripcion ?? DBNull.Value);
        var idParam = command.Parameters.Add("@CategoriaID", SqlDbType.Int);
        idParam.Direction = ParameterDirection.Output;

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
        return (int)idParam.Value;
    }

    public async Task ActualizarAsync(Categoria categoria)
    {
        await using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        await using var command = new SqlCommand("usp_Categoria_Actualizar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@CategoriaID", categoria.CategoriaID);
        command.Parameters.AddWithValue("@NombreCategoria", categoria.NombreCategoria);
        command.Parameters.AddWithValue("@Descripcion", (object?)categoria.Descripcion ?? DBNull.Value);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    public async Task EliminarAsync(int categoriaId)
    {
        await using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        await using var command = new SqlCommand("usp_Categoria_Eliminar", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.AddWithValue("@CategoriaID", categoriaId);

        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }

    private static Categoria Mapear(DataRow row)
    {
        return new Categoria
        {
            CategoriaID = row.GetInt32("CategoriaID"),
            NombreCategoria = row.GetString("NombreCategoria"),
            Descripcion = row.GetNullableString("Descripcion")
        };
    }
}
