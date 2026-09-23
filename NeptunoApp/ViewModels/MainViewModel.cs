using System.Collections.ObjectModel;
using NeptunoApp.MVVM;

namespace NeptunoApp.ViewModels;

public class MainViewModel : ViewModelBase
{
    private object? _vistaActual;
    private string _tituloPagina = "Inicio";

    public MainViewModel()
    {
        Inicio = new InicioViewModel();
        Productos = new ProductosViewModel();
        Categorias = new CategoriasViewModel();
        Proveedores = new ProveedoresViewModel();
        Pedidos = new PedidosViewModel();
        Catalogo = new CatalogoHubViewModel(Productos, Categorias, Proveedores);

        Menu.Add(new NavItem { Titulo = "Inicio", Clave = "inicio" });
        Menu.Add(new NavItem
        {
            Titulo = "Catálogo",
            Clave = "catalogo",
            Hijos =
            {
                new NavItem { Titulo = "Productos", Clave = "productos" },
                new NavItem { Titulo = "Categorías", Clave = "categorias" },
                new NavItem { Titulo = "Proveedores", Clave = "proveedores" }
            }
        });
        Menu.Add(new NavItem
        {
            Titulo = "Pedidos",
            Clave = "pedidos",
            Hijos =
            {
                new NavItem { Titulo = "Mantenimiento", Clave = "pedidos" },
                new NavItem { Titulo = "Reportes", Clave = "reportes" }
            }
        });
    }

    public InicioViewModel Inicio { get; }
    public ProductosViewModel Productos { get; }
    public CategoriasViewModel Categorias { get; }
    public ProveedoresViewModel Proveedores { get; }
    public PedidosViewModel Pedidos { get; }
    public CatalogoHubViewModel Catalogo { get; }
    public ObservableCollection<NavItem> Menu { get; } = [];

    public object? VistaActual
    {
        get => _vistaActual;
        private set => SetField(ref _vistaActual, value);
    }

    public string TituloPagina
    {
        get => _tituloPagina;
        private set => SetField(ref _tituloPagina, value);
    }

    public async Task NavegarAsync(string clave)
    {
        switch (clave)
        {
            case "inicio":
                VistaActual = Inicio;
                TituloPagina = "Inicio";
                await Inicio.CargarAsync();
                break;
            case "catalogo":
                Catalogo.TabSeleccionado = 0;
                VistaActual = Catalogo;
                TituloPagina = "Catálogo";
                await Productos.CargarAsync();
                break;
            case "productos":
                Catalogo.TabSeleccionado = 0;
                VistaActual = Catalogo;
                TituloPagina = "Productos";
                await Productos.CargarAsync();
                break;
            case "categorias":
                Catalogo.TabSeleccionado = 1;
                VistaActual = Catalogo;
                TituloPagina = "Categorías";
                await Categorias.CargarAsync();
                break;
            case "proveedores":
                Catalogo.TabSeleccionado = 2;
                VistaActual = Catalogo;
                TituloPagina = "Proveedores";
                await Proveedores.CargarAsync();
                break;
            case "pedidos":
                Pedidos.TabSeleccionado = 0;
                VistaActual = Pedidos;
                TituloPagina = "Pedidos";
                await Pedidos.CargarAsync();
                break;
            case "reportes":
                Pedidos.TabSeleccionado = 1;
                VistaActual = Pedidos;
                TituloPagina = "Reportes de pedidos";
                await Pedidos.ConsultarReporteAsync();
                break;
        }
    }
}
