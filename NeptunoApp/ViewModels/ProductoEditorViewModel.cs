using System.Collections.ObjectModel;
using Neptuno.Data;
using Neptuno.Data.Models;
using NeptunoApp.MVVM;

namespace NeptunoApp.ViewModels;

public class ProductoEditorViewModel : ViewModelBase
{
    private readonly Producto? _original;
    private string _nombreProducto = string.Empty;
    private Categoria? _categoriaSeleccionada;
    private Proveedor? _proveedorSeleccionado;
    private string? _cantidadPorUnidad;
    private decimal _precioUnidad;
    private short _unidadesEnExistencia;
    private short _unidadesEnPedido;
    private short _nivelDeReorden;
    private bool _descontinuado;
    private string _mensajeEstado = string.Empty;

    public ProductoEditorViewModel(Producto? producto = null)
    {
        _original = producto;
        EsNuevo = producto is null;
        Titulo = EsNuevo ? "Nuevo producto" : "Editar producto";
        ProductoID = producto?.ProductoID ?? 0;
        NombreProducto = producto?.NombreProducto ?? string.Empty;
        CantidadPorUnidad = producto?.CantidadPorUnidad;
        PrecioUnidad = producto?.PrecioUnidad ?? 0;
        UnidadesEnExistencia = producto?.UnidadesEnExistencia ?? 0;
        UnidadesEnPedido = producto?.UnidadesEnPedido ?? 0;
        NivelDeReorden = producto?.NivelDeReorden ?? 0;
        Descontinuado = producto?.Descontinuado ?? false;

        AceptarCommand = new RelayCommand(_ => Aceptar());
        CancelarCommand = new RelayCommand(_ => Cancelar());
    }

    public event Action<bool>? SolicitarCierre;

    public bool EsNuevo { get; }
    public string Titulo { get; }
    public int ProductoID { get; }
    public ObservableCollection<Categoria> Categorias { get; } = [];
    public ObservableCollection<Proveedor> Proveedores { get; } = [];

    public string NombreProducto
    {
        get => _nombreProducto;
        set => SetField(ref _nombreProducto, value);
    }

    public Categoria? CategoriaSeleccionada
    {
        get => _categoriaSeleccionada;
        set => SetField(ref _categoriaSeleccionada, value);
    }

    public Proveedor? ProveedorSeleccionado
    {
        get => _proveedorSeleccionado;
        set => SetField(ref _proveedorSeleccionado, value);
    }

    public string? CantidadPorUnidad
    {
        get => _cantidadPorUnidad;
        set => SetField(ref _cantidadPorUnidad, value);
    }

    public decimal PrecioUnidad
    {
        get => _precioUnidad;
        set => SetField(ref _precioUnidad, value);
    }

    public short UnidadesEnExistencia
    {
        get => _unidadesEnExistencia;
        set => SetField(ref _unidadesEnExistencia, value);
    }

    public short UnidadesEnPedido
    {
        get => _unidadesEnPedido;
        set => SetField(ref _unidadesEnPedido, value);
    }

    public short NivelDeReorden
    {
        get => _nivelDeReorden;
        set => SetField(ref _nivelDeReorden, value);
    }

    public bool Descontinuado
    {
        get => _descontinuado;
        set => SetField(ref _descontinuado, value);
    }

    public string MensajeEstado
    {
        get => _mensajeEstado;
        private set => SetField(ref _mensajeEstado, value);
    }

    public RelayCommand AceptarCommand { get; }
    public RelayCommand CancelarCommand { get; }

    public async Task CargarCombosAsync()
    {
        var categorias = await new CategoriaRepository().ListarAsync();
        categorias.Insert(0, new Categoria { CategoriaID = 0, NombreCategoria = "(Sin categoría)" });
        Categorias.Clear();
        foreach (var categoria in categorias)
        {
            Categorias.Add(categoria);
        }

        var proveedores = await new ProveedorRepository().ListarAsync();
        proveedores.Insert(0, new Proveedor { ProveedorID = 0, CompaniaNombre = "(Sin proveedor)" });
        Proveedores.Clear();
        foreach (var proveedor in proveedores)
        {
            Proveedores.Add(proveedor);
        }

        CategoriaSeleccionada = Categorias.FirstOrDefault(c => c.CategoriaID == (_original?.CategoriaID ?? 0));
        ProveedorSeleccionado = Proveedores.FirstOrDefault(p => p.ProveedorID == (_original?.ProveedorID ?? 0));
    }

    public Producto ToModelo()
    {
        return new Producto
        {
            ProductoID = ProductoID,
            NombreProducto = NombreProducto.Trim(),
            CategoriaID = CategoriaSeleccionada is { CategoriaID: > 0 } ? CategoriaSeleccionada.CategoriaID : null,
            ProveedorID = ProveedorSeleccionado is { ProveedorID: > 0 } ? ProveedorSeleccionado.ProveedorID : null,
            CantidadPorUnidad = string.IsNullOrWhiteSpace(CantidadPorUnidad) ? null : CantidadPorUnidad.Trim(),
            PrecioUnidad = PrecioUnidad,
            UnidadesEnExistencia = UnidadesEnExistencia,
            UnidadesEnPedido = UnidadesEnPedido,
            NivelDeReorden = NivelDeReorden,
            Descontinuado = Descontinuado
        };
    }

    private void Aceptar()
    {
        if (string.IsNullOrWhiteSpace(NombreProducto))
        {
            MensajeEstado = "El nombre del producto es obligatorio.";
            return;
        }

        if (PrecioUnidad < 0)
        {
            MensajeEstado = "El precio no puede ser negativo.";
            return;
        }

        SolicitarCierre?.Invoke(true);
    }

    private void Cancelar() => SolicitarCierre?.Invoke(false);
}
