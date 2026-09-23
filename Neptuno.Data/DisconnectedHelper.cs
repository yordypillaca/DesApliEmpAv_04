using System.Data;
using Microsoft.Data.SqlClient;

namespace Neptuno.Data;

/// <summary>
/// Modo DESCONECTADO: SqlDataAdapter.Fill carga un DataSet en memoria
/// y la conexión se cierra al terminar. Las consultas (listados/reportes)
/// trabajan sobre ese DataSet, no sobre un DataReader abierto.
/// </summary>
internal static class DisconnectedHelper
{
    public static async Task<DataSet> FillAsync(string storedProcedure, Action<SqlCommand>? configure = null)
    {
        var dataSet = new DataSet();
        await using var connection = new SqlConnection(ConnectionHelper.DatabaseConnectionString);
        await using var command = new SqlCommand(storedProcedure, connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        configure?.Invoke(command);

        using var adapter = new SqlDataAdapter(command);
        await connection.OpenAsync();
        adapter.Fill(dataSet);
        return dataSet;
    }

    public static DataTable FirstTable(DataSet dataSet)
    {
        if (dataSet.Tables.Count == 0)
        {
            return new DataTable();
        }

        return dataSet.Tables[0];
    }

    public static IEnumerable<DataRow> Rows(DataTable table)
    {
        foreach (DataRow row in table.Rows)
        {
            yield return row;
        }
    }
}
