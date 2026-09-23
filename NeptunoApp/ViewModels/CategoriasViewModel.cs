using System.Collections.ObjectModel;
using Neptuno.Data;
using Neptuno.Data.Models;
using NeptunoApp.Helpers;
using NeptunoApp.MVVM;

namespace NeptunoApp.ViewModels;

public class CategoriasViewModel : ViewModelBase
{
    private readonly CategoriaRepository _repositorio = new();
    private Categoria? _seleccionada;
    private string _mensajeEstado = string.Empty;

    public CategoriasViewModel()
    {
        CargarCommand = new AsyncRelayCommand(_ => CargarAsync());
        NuevoCommand = new AsyncRelayCommand(_ => NuevoAsync());
        EditarCommand = new AsyncRelayCommand(_ => EditarAsync(), _ => Seleccionada is not null);
        EliminarCommand = new AsyncRelayCommand(_ => EliminarAsync(), _ => Seleccionada is not null);
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

    public AsyncRelayCommand CargarCommand { get; }
    public AsyncRelayCommand NuevoCommand { get; }
    public AsyncRelayCommand EditarCommand { get; }
    public AsyncRelayCommand EliminarCommand { get; }

    public async Task CargarAsync()
    {
        try
        {
            var lista = await _repositorio.ListarAsync();
            Categorias.Clear();
            foreach (var categoria in lista)
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

    private async Task NuevoAsync()
    {
        var editor = new CategoriaEditorViewModel();
        if (await DialogService.EditarCategoriaAsync(editor) != true)
        {
            return;
        }

        try
        {
            await _repositorio.InsertarAsync(editor.ToModelo());
            await CargarAsync();
            MensajeEstado = "Categoría registrada.";
        }
        catch (Exception ex)
        {
            DialogService.Error(ex.Message);
        }
    }

    private async Task EditarAsync()
    {
        if (Seleccionada is null)
        {
            return;
        }

        var editor = new CategoriaEditorViewModel(Seleccionada);
        if (await DialogService.EditarCategoriaAsync(editor) != true)
        {
            return;
        }

        try
        {
            await _repositorio.ActualizarAsync(editor.ToModelo());
            await CargarAsync();
            MensajeEstado = "Categoría actualizada.";
        }
        catch (Exception ex)
        {
            DialogService.Error(ex.Message);
        }
    }

    private async Task EliminarAsync()
    {
        if (Seleccionada is null || !DialogService.Confirmar($"¿Dar de baja la categoría \"{Seleccionada.NombreCategoria}\"? El registro no se borra, solo se marca como inactivo."))
        {
            return;
        }

        try
        {
            await _repositorio.EliminarAsync(Seleccionada.CategoriaID);
            await CargarAsync();
            MensajeEstado = "Categoría dada de baja (eliminación lógica).";
        }
        catch (Exception ex)
        {
            DialogService.Error(ex.Message);
        }
    }
}
