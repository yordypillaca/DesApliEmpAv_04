using System.Collections.ObjectModel;
using NeptunoApp.Data;
using NeptunoApp.Helpers;
using NeptunoApp.Models;
using NeptunoApp.MVVM;

namespace NeptunoApp.ViewModels;

public class CategoriasViewModel : ViewModelBase
{
    private readonly CategoriaRepository _repositorio = new();
    private Categoria? _seleccionada;
    private string _mensajeEstado = string.Empty;

    public CategoriasViewModel()
    {
        CargarCommand = new RelayCommand(_ => Cargar());
        NuevoCommand = new RelayCommand(_ => Nuevo());
        EditarCommand = new RelayCommand(_ => Editar(), _ => Seleccionada is not null);
        EliminarCommand = new RelayCommand(_ => Eliminar(), _ => Seleccionada is not null);
        Cargar();
    }

    public ObservableCollection<Categoria> Categorias { get; } = [];

    public Categoria? Seleccionada
    {
        get => _seleccionada;
        set => SetField(ref _seleccionada, value);
    }

    public string MensajeEstado
    {
        get => _mensajeEstado;
        private set => SetField(ref _mensajeEstado, value);
    }

    public RelayCommand CargarCommand { get; }
    public RelayCommand NuevoCommand { get; }
    public RelayCommand EditarCommand { get; }
    public RelayCommand EliminarCommand { get; }

    public void Cargar()
    {
        try
        {
            Categorias.Clear();
            foreach (var categoria in _repositorio.Listar())
            {
                Categorias.Add(categoria);
            }

            MensajeEstado = $"{Categorias.Count} categoría(s) cargada(s).";
        }
        catch (Exception ex)
        {
            MensajeEstado = $"Error al cargar: {ex.Message}";
        }
    }

    private void Nuevo()
    {
        var editor = new CategoriaEditorViewModel();
        if (DialogService.EditarCategoria(editor) != true)
        {
            return;
        }

        try
        {
            _repositorio.Insertar(editor.ToModelo());
            Cargar();
            MensajeEstado = "Categoría registrada.";
        }
        catch (Exception ex)
        {
            DialogService.Error(ex.Message);
        }
    }

    private void Editar()
    {
        if (Seleccionada is null)
        {
            return;
        }

        var editor = new CategoriaEditorViewModel(Seleccionada);
        if (DialogService.EditarCategoria(editor) != true)
        {
            return;
        }

        try
        {
            _repositorio.Actualizar(editor.ToModelo());
            Cargar();
            MensajeEstado = "Categoría actualizada.";
        }
        catch (Exception ex)
        {
            DialogService.Error(ex.Message);
        }
    }

    private void Eliminar()
    {
        if (Seleccionada is null || !DialogService.Confirmar($"¿Dar de baja la categoría \"{Seleccionada.NombreCategoria}\"? El registro no se borra, solo se marca como inactivo."))
        {
            return;
        }

        try
        {
            _repositorio.Eliminar(Seleccionada.CategoriaID);
            Cargar();
            MensajeEstado = "Categoría dada de baja (eliminación lógica).";
        }
        catch (Exception ex)
        {
            DialogService.Error(ex.Message);
        }
    }
}
