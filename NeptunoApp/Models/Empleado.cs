namespace NeptunoApp.Models;

public class Empleado
{
    public int EmpleadoID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string? Cargo { get; set; }
    public string? Ciudad { get; set; }
    public string? Pais { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
}
