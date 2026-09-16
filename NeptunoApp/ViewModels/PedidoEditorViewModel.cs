using System.Collections.ObjectModel;
using NeptunoApp.Data;
using NeptunoApp.Models;
using NeptunoApp.MVVM;

namespace NeptunoApp.ViewModels;

public class PedidoEditorViewModel : ViewModelBase
{
    private Cliente? _clienteSeleccionado;
    private Empleado? _empleadoSeleccionado;
    private Transportista? _transportistaSeleccionado;
    private DateTime _fechaPedido = DateTime.Today;
    private DateTime? _fechaRequerida = DateTime.Today.AddDays(10);
    private DateTime? _fechaEnvio;
    private string? _destinatario;
    private string? _ciudadDestino;
    private string? _paisDestino = "Perú";
    private Producto? _productoParaAgregar;
    private short _cantidadNueva = 1;
    private decimal _descuentoNuevo;
    private DetallePedido? _detalleSeleccionado;
    private string _mensajeEstado = string.Empty;

    public PedidoEditorViewModel(Pedido? pedido = null)
    {
        EsNuevo = pedido is null;
        Titulo = EsNuevo ? "Nuevo pedido" : $"Editar pedido #{pedido!.PedidoID}";
        PedidoID = pedido?.PedidoID ?? 0;

        var catalogo = new CatalogoRepository();
        Clientes = new ObservableCollection<Cliente>(catalogo.ListarClientes());
        Empleados = new ObservableCollection<Empleado>(catalogo.ListarEmpleados());
        Transportistas = new ObservableCollection<Transportista>(catalogo.ListarTransportistas());
        Productos = new ObservableCollection<Producto>(new ProductoRepository().Listar());

        if (pedido is not null)
        {
            ClienteSeleccionado = Clientes.FirstOrDefault(c => c.ClienteID == pedido.ClienteID);
            EmpleadoSeleccionado = Empleados.FirstOrDefault(e => e.EmpleadoID == pedido.EmpleadoID);
            TransportistaSeleccionado = Transportistas.FirstOrDefault(t => t.TransportistaID == pedido.TransportistaID);
            FechaPedido = pedido.FechaPedido;
            FechaRequerida = pedido.FechaRequerida;
            FechaEnvio = pedido.FechaEnvio;
            Destinatario = pedido.Destinatario;
            CiudadDestino = pedido.CiudadDestino;
            PaisDestino = pedido.PaisDestino;

            foreach (var linea in new PedidoRepository().ListarDetalles(pedido.PedidoID))
            {
                Detalles.Add(linea);
            }
        }
        else
        {
            ClienteSeleccionado = Clientes.FirstOrDefault();
            EmpleadoSeleccionado = Empleados.FirstOrDefault();
            TransportistaSeleccionado = Transportistas.FirstOrDefault();
            Destinatario = ClienteSeleccionado?.Empresa;
            CiudadDestino = ClienteSeleccionado?.Ciudad;
            PaisDestino = ClienteSeleccionado?.Pais ?? "Perú";
        }

        ProductoParaAgregar = Productos.FirstOrDefault();

        AgregarLineaCommand = new RelayCommand(_ => AgregarLinea(), _ => ProductoParaAgregar is not null && CantidadNueva > 0);
        QuitarLineaCommand = new RelayCommand(_ => QuitarLinea(), _ => DetalleSeleccionado is not null);
        AceptarCommand = new RelayCommand(_ => Aceptar());
        CancelarCommand = new RelayCommand(_ => Cancelar());
    }

    public event Action<bool>? SolicitarCierre;

    public bool EsNuevo { get; }
    public string Titulo { get; }
    public int PedidoID { get; }
    public ObservableCollection<Cliente> Clientes { get; }
    public ObservableCollection<Empleado> Empleados { get; }
    public ObservableCollection<Transportista> Transportistas { get; }
    public ObservableCollection<Producto> Productos { get; }
    public ObservableCollection<DetallePedido> Detalles { get; } = [];

    public Cliente? ClienteSeleccionado
    {
        get => _clienteSeleccionado;
        set
        {
            if (!SetField(ref _clienteSeleccionado, value) || value is null)
            {
                return;
            }

            Destinatario = value.Empresa;
            CiudadDestino = value.Ciudad;
            PaisDestino = value.Pais;
        }
    }

    public Empleado? EmpleadoSeleccionado
    {
        get => _empleadoSeleccionado;
        set => SetField(ref _empleadoSeleccionado, value);
    }

    public Transportista? TransportistaSeleccionado
    {
        get => _transportistaSeleccionado;
        set => SetField(ref _transportistaSeleccionado, value);
    }

    public DateTime FechaPedido { get => _fechaPedido; set => SetField(ref _fechaPedido, value); }
    public DateTime? FechaRequerida { get => _fechaRequerida; set => SetField(ref _fechaRequerida, value); }
    public DateTime? FechaEnvio { get => _fechaEnvio; set => SetField(ref _fechaEnvio, value); }
    public string? Destinatario { get => _destinatario; set => SetField(ref _destinatario, value); }
    public string? CiudadDestino { get => _ciudadDestino; set => SetField(ref _ciudadDestino, value); }
    public string? PaisDestino { get => _paisDestino; set => SetField(ref _paisDestino, value); }

    public Producto? ProductoParaAgregar
    {
        get => _productoParaAgregar;
        set => SetField(ref _productoParaAgregar, value);
    }

    public short CantidadNueva
    {
        get => _cantidadNueva;
        set => SetField(ref _cantidadNueva, value);
    }

    public decimal DescuentoNuevo
    {
        get => _descuentoNuevo;
        set => SetField(ref _descuentoNuevo, value);
    }

    public DetallePedido? DetalleSeleccionado
    {
        get => _detalleSeleccionado;
        set => SetField(ref _detalleSeleccionado, value);
    }

    public string MensajeEstado
    {
        get => _mensajeEstado;
        private set => SetField(ref _mensajeEstado, value);
    }

    public decimal Total => Detalles.Sum(d => d.Importe);

    public RelayCommand AgregarLineaCommand { get; }
    public RelayCommand QuitarLineaCommand { get; }
    public RelayCommand AceptarCommand { get; }
    public RelayCommand CancelarCommand { get; }

    public Pedido ToModelo()
    {
        return new Pedido
        {
            PedidoID = PedidoID,
            ClienteID = ClienteSeleccionado?.ClienteID,
            EmpleadoID = EmpleadoSeleccionado?.EmpleadoID,
            TransportistaID = TransportistaSeleccionado?.TransportistaID,
            FechaPedido = FechaPedido,
            FechaRequerida = FechaRequerida,
            FechaEnvio = FechaEnvio,
            Destinatario = string.IsNullOrWhiteSpace(Destinatario) ? null : Destinatario.Trim(),
            CiudadDestino = string.IsNullOrWhiteSpace(CiudadDestino) ? null : CiudadDestino.Trim(),
            PaisDestino = string.IsNullOrWhiteSpace(PaisDestino) ? null : PaisDestino.Trim()
        };
    }

    private void AgregarLinea()
    {
        if (ProductoParaAgregar is null)
        {
            return;
        }

        if (Detalles.Any(d => d.ProductoID == ProductoParaAgregar.ProductoID))
        {
            MensajeEstado = "Ese producto ya está en el detalle. Quítalo si deseas cambiarlo.";
            return;
        }

        if (CantidadNueva <= 0)
        {
            MensajeEstado = "La cantidad debe ser mayor a cero.";
            return;
        }

        if (DescuentoNuevo is < 0 or > 1)
        {
            MensajeEstado = "El descuento debe estar entre 0 y 1 (por ejemplo 0.10 = 10%).";
            return;
        }

        var linea = new DetallePedido
        {
            ProductoID = ProductoParaAgregar.ProductoID,
            NombreProducto = ProductoParaAgregar.NombreProducto,
            PrecioUnidad = ProductoParaAgregar.PrecioUnidad,
            Cantidad = CantidadNueva,
            Descuento = DescuentoNuevo,
            Importe = Math.Round(ProductoParaAgregar.PrecioUnidad * CantidadNueva * (1 - DescuentoNuevo), 2)
        };

        Detalles.Add(linea);
        OnPropertyChanged(nameof(Total));
        CantidadNueva = 1;
        DescuentoNuevo = 0;
        MensajeEstado = $"Se agregó {linea.NombreProducto}.";
    }

    private void QuitarLinea()
    {
        if (DetalleSeleccionado is null)
        {
            return;
        }

        Detalles.Remove(DetalleSeleccionado);
        OnPropertyChanged(nameof(Total));
        MensajeEstado = "Línea eliminada del detalle.";
    }

    private void Aceptar()
    {
        if (ClienteSeleccionado is null)
        {
            MensajeEstado = "Selecciona un cliente.";
            return;
        }

        if (Detalles.Count == 0)
        {
            MensajeEstado = "Agrega al menos un producto al pedido.";
            return;
        }

        SolicitarCierre?.Invoke(true);
    }

    private void Cancelar() => SolicitarCierre?.Invoke(false);
}
