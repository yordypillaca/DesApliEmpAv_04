namespace Neptuno.Data.Models;

public class Cliente
{
    public int ClienteID { get; set; }
    public string Empresa { get; set; } = string.Empty;
    public string? NombreContacto { get; set; }
    public string? Ciudad { get; set; }
    public string? Pais { get; set; }
    public string? Telefono { get; set; }
}
