using Microsoft.Data.SqlClient;

namespace NeptunoApp.Data;

internal static class SqlReaderExtensions
{
    public static string? GetNullableString(this SqlDataReader reader, int ordinal)
        => reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);

    public static int? GetNullableInt32(this SqlDataReader reader, int ordinal)
        => reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);

    public static DateTime? GetNullableDateTime(this SqlDataReader reader, int ordinal)
        => reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);

    public static short GetInt16Safe(this SqlDataReader reader, int ordinal)
        => reader.IsDBNull(ordinal) ? (short)0 : reader.GetInt16(ordinal);
}
