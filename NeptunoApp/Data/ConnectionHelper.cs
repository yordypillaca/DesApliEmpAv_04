using System.Configuration;

namespace NeptunoApp.Data;

public static class ConnectionHelper
{
    public static string DatabaseConnectionString =>
        ConfigurationManager.ConnectionStrings["NeptunoDB"].ConnectionString;
}
