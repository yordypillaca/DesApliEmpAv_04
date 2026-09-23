using System.Collections.ObjectModel;
using Neptuno.Data;
using Neptuno.Data.Models;
using NeptunoApp.MVVM;

namespace NeptunoApp.ViewModels;

/// <summary>
/// Recorre las categorías del catálogo.
/// </summary>
public class InicioViewModel : ViewModelBase
{
    private int _indiceActual;
    private string _mensajeEstado = string.Empty;

    public InicioViewModel()
    {
        AnteriorCommand = new RelayCommand(_ => Mover(-1), _ => Slides.Count > 0);
        SiguienteCommand = new RelayCommand(_ => Mover(1), _ => Slides.Count > 0);
    }

    public ObservableCollection<CategoriaCarrusel> Slides { get; } = [];

    public int IndiceActual
    {
        get => _indiceActual;
        private set
        {
            if (SetField(ref _indiceActual, value))
            {
                OnPropertyChanged(nameof(SlideActual));
                OnPropertyChanged(nameof(Indicador));
            }
        }
    }

    public CategoriaCarrusel? SlideActual => Slides.Count == 0 ? null : Slides[IndiceActual];

    public string Indicador => Slides.Count == 0 ? "0 / 0" : $"{IndiceActual + 1} / {Slides.Count}";

    public string MensajeEstado
    {
        get => _mensajeEstado;
        private set => SetField(ref _mensajeEstado, value);
    }

    public RelayCommand AnteriorCommand { get; }
    public RelayCommand SiguienteCommand { get; }

    public async Task CargarAsync()
    {
        try
        {
            var categorias = await new CategoriaRepository().ListarAsync();
            var productos = await new ProductoRepository().ListarAsync();
            var pedidos = await new PedidoRepository().ListarAsync();

            Slides.Clear();
            foreach (var categoria in categorias)
            {
                var deCategoria = productos.Where(p => p.CategoriaID == categoria.CategoriaID).ToList();
                Slides.Add(new CategoriaCarrusel
                {
                    Categoria = categoria,
                    CantidadProductos = deCategoria.Count,
                    ProductosDestacados = string.Join(" · ", deCategoria.Select(p => p.NombreProducto))
                });
            }

            IndiceActual = 0;
            MensajeEstado = $"{categorias.Count} categorías · {productos.Count} productos · {pedidos.Count} pedidos";
        }
        catch (Exception ex)
        {
            MensajeEstado = $"No se pudo cargar el inicio: {ex.Message}";
        }
    }

    private void Mover(int delta)
    {
        if (Slides.Count == 0)
        {
            return;
        }

        var siguiente = (IndiceActual + delta) % Slides.Count;
        if (siguiente < 0)
        {
            siguiente += Slides.Count;
        }

        IndiceActual = siguiente;
    }
}

public class CategoriaCarrusel
{
    public Categoria Categoria { get; set; } = new();
    public int CantidadProductos { get; set; }
    public string ProductosDestacados { get; set; } = string.Empty;
}
