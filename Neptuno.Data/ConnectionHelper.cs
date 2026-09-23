using System.Configuration;

namespace Neptuno.Data;

/// <summary>
/// Lee la cadena de conexión del App.config del proyecto de INICIO (WPF).
/// Gotcha: ConfigurationManager no usa un App.config de la Class Library;
/// la cadena debe vivir en NeptunoApp/App.config.
/// </summary>
public static class ConnectionHelper
{
    public static string DatabaseConnectionString =>
        ConfigurationManager.ConnectionStrings["NeptunoDB"]?.ConnectionString
        ?? throw new InvalidOperationException(
            "No se encontró la cadena 'NeptunoDB' en el App.config del proyecto de inicio (WPF).");
}
