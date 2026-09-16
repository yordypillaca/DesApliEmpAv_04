using NeptunoApp.Models;
using NeptunoApp.MVVM;

namespace NeptunoApp.ViewModels;

public class CategoriaEditorViewModel : ViewModelBase
{
    private string _nombreCategoria = string.Empty;
    private string? _descripcion;
    private string _mensajeEstado = string.Empty;

    public CategoriaEditorViewModel(Categoria? categoria = null)
    {
        EsNuevo = categoria is null;
        Titulo = EsNuevo ? "Nueva categoría" : "Editar categoría";
        CategoriaID = categoria?.CategoriaID ?? 0;
        NombreCategoria = categoria?.NombreCategoria ?? string.Empty;
        Descripcion = categoria?.Descripcion;

        AceptarCommand = new RelayCommand(_ => Aceptar());
        CancelarCommand = new RelayCommand(_ => Cancelar());
    }

    public event Action<bool>? SolicitarCierre;

    public bool EsNuevo { get; }
    public string Titulo { get; }
    public int CategoriaID { get; }

    public string NombreCategoria
    {
        get => _nombreCategoria;
        set => SetField(ref _nombreCategoria, value);
    }

    public string? Descripcion
    {
        get => _descripcion;
        set => SetField(ref _descripcion, value);
    }

    public string MensajeEstado
    {
        get => _mensajeEstado;
        private set => SetField(ref _mensajeEstado, value);
    }

    public RelayCommand AceptarCommand { get; }
    public RelayCommand CancelarCommand { get; }

    public Categoria ToModelo()
    {
        return new Categoria
        {
            CategoriaID = CategoriaID,
            NombreCategoria = NombreCategoria.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(Descripcion) ? null : Descripcion.Trim()
        };
    }

    private void Aceptar()
    {
        if (string.IsNullOrWhiteSpace(NombreCategoria))
        {
            MensajeEstado = "El nombre de la categoría es obligatorio.";
            return;
        }

        SolicitarCierre?.Invoke(true);
    }

    private void Cancelar() => SolicitarCierre?.Invoke(false);
}
