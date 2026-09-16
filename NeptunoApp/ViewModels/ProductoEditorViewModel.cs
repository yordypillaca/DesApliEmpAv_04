using System.Collections.ObjectModel;
using NeptunoApp.Data;
using NeptunoApp.Models;
using NeptunoApp.MVVM;

namespace NeptunoApp.ViewModels;

public class ProductoEditorViewModel : ViewModelBase
{
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
        EsNuevo = producto is null;
        Titulo = EsNuevo ? "Nuevo producto" : "Editar producto";
        ProductoID = producto?.ProductoID ?? 0;

        var categorias = new CategoriaRepository().Listar();
        categorias.Insert(0, new Categoria { CategoriaID = 0, NombreCategoria = "(Sin categoría)" });
        Categorias = new ObservableCollection<Categoria>(categorias);

        var proveedores = new ProveedorRepository().Listar();
        proveedores.Insert(0, new Proveedor { ProveedorID = 0, CompaniaNombre = "(Sin proveedor)" });
        Proveedores = new ObservableCollection<Proveedor>(proveedores);

        if (producto is not null)
        {
            NombreProducto = producto.NombreProducto;
            CategoriaSeleccionada = Categorias.FirstOrDefault(c => c.CategoriaID == (producto.CategoriaID ?? 0));
            ProveedorSeleccionado = Proveedores.FirstOrDefault(p => p.ProveedorID == (producto.ProveedorID ?? 0));
            CantidadPorUnidad = producto.CantidadPorUnidad;
            PrecioUnidad = producto.PrecioUnidad;
            UnidadesEnExistencia = producto.UnidadesEnExistencia;
            UnidadesEnPedido = producto.UnidadesEnPedido;
            NivelDeReorden = producto.NivelDeReorden;
            Descontinuado = producto.Descontinuado;
        }
        else
        {
            CategoriaSeleccionada = Categorias[0];
            ProveedorSeleccionado = Proveedores[0];
        }

        AceptarCommand = new RelayCommand(_ => Aceptar());
        CancelarCommand = new RelayCommand(_ => Cancelar());
    }

    public event Action<bool>? SolicitarCierre;

    public bool EsNuevo { get; }
    public string Titulo { get; }
    public int ProductoID { get; }
    public ObservableCollection<Categoria> Categorias { get; }
    public ObservableCollection<Proveedor> Proveedores { get; }

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
