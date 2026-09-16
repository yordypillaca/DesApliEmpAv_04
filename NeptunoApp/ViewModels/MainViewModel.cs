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

        Navegar("inicio");
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

    public void Navegar(string clave)
    {
        switch (clave)
        {
            case "inicio":
                Inicio.Cargar();
                VistaActual = Inicio;
                TituloPagina = "Inicio";
                break;
            case "catalogo":
                Catalogo.TabSeleccionado = 0;
                Productos.Cargar();
                VistaActual = Catalogo;
                TituloPagina = "Catálogo";
                break;
            case "productos":
                Catalogo.TabSeleccionado = 0;
                Productos.Cargar();
                VistaActual = Catalogo;
                TituloPagina = "Productos";
                break;
            case "categorias":
                Catalogo.TabSeleccionado = 1;
                Categorias.Cargar();
                VistaActual = Catalogo;
                TituloPagina = "Categorías";
                break;
            case "proveedores":
                Catalogo.TabSeleccionado = 2;
                Proveedores.Cargar();
                VistaActual = Catalogo;
                TituloPagina = "Proveedores";
                break;
            case "pedidos":
                Pedidos.TabSeleccionado = 0;
                Pedidos.Cargar();
                VistaActual = Pedidos;
                TituloPagina = "Pedidos";
                break;
            case "reportes":
                Pedidos.TabSeleccionado = 1;
                Pedidos.ConsultarReporte();
                VistaActual = Pedidos;
                TituloPagina = "Reportes de pedidos";
                break;
        }
    }
}
