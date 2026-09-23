using System.Collections.ObjectModel;
using Neptuno.Data;
using Neptuno.Data.Models;
using NeptunoApp.Helpers;
using NeptunoApp.MVVM;

namespace NeptunoApp.ViewModels;

public class ProductosViewModel : ViewModelBase
{
    private readonly ProductoRepository _repositorio = new();
    private Producto? _seleccionado;
    private string _mensajeEstado = string.Empty;

    public ProductosViewModel()
    {
        CargarCommand = new AsyncRelayCommand(_ => CargarAsync());
        NuevoCommand = new AsyncRelayCommand(_ => NuevoAsync());
        EditarCommand = new AsyncRelayCommand(_ => EditarAsync(), _ => Seleccionado is not null);
        EliminarCommand = new AsyncRelayCommand(_ => EliminarAsync(), _ => Seleccionado is not null);
    }

    public ObservableCollection<Producto> Productos { get; } = [];

    public Producto? Seleccionado
    {
        get => _seleccionado;
        set => SetField(ref _seleccionado, value);
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
            Productos.Clear();
            foreach (var producto in lista)
            {
                Productos.Add(producto);
            }

            MensajeEstado = $"{Productos.Count} producto(s) cargado(s).";
        }
        catch (Exception ex)
        {
            MensajeEstado = $"Error al cargar: {ex.Message}";
        }
    }

    private async Task NuevoAsync()
    {
        var editor = new ProductoEditorViewModel();
        if (await DialogService.EditarProductoAsync(editor) != true)
        {
            return;
        }

        try
        {
            await _repositorio.InsertarAsync(editor.ToModelo());
            await CargarAsync();
            MensajeEstado = "Producto registrado.";
        }
        catch (Exception ex)
        {
            DialogService.Error(ex.Message);
        }
    }

    private async Task EditarAsync()
    {
        if (Seleccionado is null)
        {
            return;
        }

        var editor = new ProductoEditorViewModel(Seleccionado);
        if (await DialogService.EditarProductoAsync(editor) != true)
        {
            return;
        }

        try
        {
            await _repositorio.ActualizarAsync(editor.ToModelo());
            await CargarAsync();
            MensajeEstado = "Producto actualizado.";
        }
        catch (Exception ex)
        {
            DialogService.Error(ex.Message);
        }
    }

    private async Task EliminarAsync()
    {
        if (Seleccionado is null || !DialogService.Confirmar($"¿Dar de baja el producto \"{Seleccionado.NombreProducto}\"? El registro no se borra, solo se marca como inactivo."))
        {
            return;
        }

        try
        {
            await _repositorio.EliminarAsync(Seleccionado.ProductoID);
            await CargarAsync();
            MensajeEstado = "Producto dado de baja (eliminación lógica).";
        }
        catch (Exception ex)
        {
            DialogService.Error(ex.Message);
        }
    }
}
