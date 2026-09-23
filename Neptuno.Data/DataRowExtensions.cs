using System.Data;

namespace Neptuno.Data;

internal static class DataRowExtensions
{
    public static bool IsNull(this DataRow row, string column)
        => row[column] is DBNull or null;

    public static string GetString(this DataRow row, string column)
        => Convert.ToString(row[column]) ?? string.Empty;

    public static string? GetNullableString(this DataRow row, string column)
        => row.IsNull(column) ? null : Convert.ToString(row[column]);

    public static int GetInt32(this DataRow row, string column)
        => Convert.ToInt32(row[column]);

    public static int? GetNullableInt32(this DataRow row, string column)
        => row.IsNull(column) ? null : Convert.ToInt32(row[column]);

    public static short GetInt16(this DataRow row, string column)
        => Convert.ToInt16(row[column]);

    public static decimal GetDecimal(this DataRow row, string column)
        => Convert.ToDecimal(row[column]);

    public static bool GetBoolean(this DataRow row, string column)
        => Convert.ToBoolean(row[column]);

    public static DateTime GetDateTime(this DataRow row, string column)
        => Convert.ToDateTime(row[column]);

    public static DateTime? GetNullableDateTime(this DataRow row, string column)
        => row.IsNull(column) ? null : Convert.ToDateTime(row[column]);
}
