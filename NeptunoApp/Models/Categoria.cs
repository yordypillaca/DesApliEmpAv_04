namespace NeptunoApp.Models;

public class Categoria
{
    public int CategoriaID { get; set; }
    public string NombreCategoria { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
