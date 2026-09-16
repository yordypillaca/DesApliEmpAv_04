using NeptunoApp.MVVM;

namespace NeptunoApp.ViewModels;

/// <summary>
/// Contenedor del catálogo: productos, categorías y proveedores.
/// </summary>
public class CatalogoHubViewModel : ViewModelBase
{
    private int _tabSeleccionado;

    public CatalogoHubViewModel(ProductosViewModel productos, CategoriasViewModel categorias, ProveedoresViewModel proveedores)
    {
        Productos = productos;
        Categorias = categorias;
        Proveedores = proveedores;
    }

    public ProductosViewModel Productos { get; }
    public CategoriasViewModel Categorias { get; }
    public ProveedoresViewModel Proveedores { get; }

    public int TabSeleccionado
    {
        get => _tabSeleccionado;
        set => SetField(ref _tabSeleccionado, value);
    }
}
