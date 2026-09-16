namespace NeptunoApp.Models;

public class DetallePedido
{
    public int PedidoID { get; set; }
    public int ProductoID { get; set; }
    public decimal PrecioUnidad { get; set; }
    public short Cantidad { get; set; }
    public decimal Descuento { get; set; }
    public string? NombreProducto { get; set; }
    public DateTime? FechaPedido { get; set; }
    public string? Destinatario { get; set; }
    public string? CiudadDestino { get; set; }
    public decimal Importe { get; set; }
}
