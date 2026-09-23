namespace Neptuno.Data.Models;

public class Pedido
{
    public int PedidoID { get; set; }
    public int? ClienteID { get; set; }
    public int? EmpleadoID { get; set; }
    public DateTime FechaPedido { get; set; }
    public DateTime? FechaRequerida { get; set; }
    public DateTime? FechaEnvio { get; set; }
    public int? TransportistaID { get; set; }
    public string? Destinatario { get; set; }
    public string? CiudadDestino { get; set; }
    public string? PaisDestino { get; set; }
    public string? NombreCliente { get; set; }
    public string? NombreEmpleado { get; set; }
    public string? NombreTransportista { get; set; }
}
