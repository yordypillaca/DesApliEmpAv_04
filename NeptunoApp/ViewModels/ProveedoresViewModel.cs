using System.Collections.ObjectModel;
using Neptuno.Data;
using Neptuno.Data.Models;
using NeptunoApp.Helpers;
using NeptunoApp.MVVM;

namespace NeptunoApp.ViewModels;

public class ProveedoresViewModel : ViewModelBase
{
    private readonly ProveedorRepository _repositorio = new();
    private Proveedor? _seleccionado;
    private string _filtroContacto = string.Empty;
    private string _filtroCiudad = string.Empty;
    private string _mensajeEstado = string.Empty;

    public ProveedoresViewModel()
    {
        CargarCommand = new AsyncRelayCommand(_ => CargarAsync());
        BuscarCommand = new AsyncRelayCommand(_ => BuscarAsync());
        LimpiarCommand = new AsyncRelayCommand(_ => LimpiarAsync());
        NuevoCommand = new AsyncRelayCommand(_ => NuevoAsync());
        EditarCommand = new AsyncRelayCommand(_ => EditarAsync(), _ => Seleccionado is not null);
        EliminarCommand = new AsyncRelayCommand(_ => EliminarAsync(), _ => Seleccionado is not null);
    }

    public ObservableCollection<Proveedor> Proveedores { get; } = [];

    public Proveedor? Seleccionado
    {
        get => _seleccionado;
        set => SetField(ref _seleccionado, value);
    }

    public string FiltroContacto
    {
        get => _filtroContacto;
        set => SetField(ref _filtroContacto, value);
    }

    public string FiltroCiudad
    {
        get => _filtroCiudad;
        set => SetField(ref _filtroCiudad, value);
    }

    public string MensajeEstado
    {
        get => _mensajeEstado;
        private set => SetField(ref _mensajeEstado, value);
    }

    public AsyncRelayCommand CargarCommand { get; }
    public AsyncRelayCommand BuscarCommand { get; }
    public AsyncRelayCommand LimpiarCommand { get; }
    public AsyncRelayCommand NuevoCommand { get; }
    public AsyncRelayCommand EditarCommand { get; }
    public AsyncRelayCommand EliminarCommand { get; }

    public async Task CargarAsync()
    {
        try
        {
            Reemplazar(await _repositorio.ListarAsync());
            MensajeEstado = $"{Proveedores.Count} proveedor(es) cargado(s).";
        }
        catch (Exception ex)
        {
            MensajeEstado = $"Error al cargar: {ex.Message}";
        }
    }

    private async Task BuscarAsync()
    {
        try
        {
            Reemplazar(await _repositorio.BuscarAsync(FiltroContacto, FiltroCiudad));
            MensajeEstado = $"{Proveedores.Count} proveedor(es) encontrados con los filtros.";
        }
        catch (Exception ex)
        {
            MensajeEstado = $"Error al buscar: {ex.Message}";
        }
    }

    private async Task LimpiarAsync()
    {
        FiltroContacto = string.Empty;
        FiltroCiudad = string.Empty;
        await CargarAsync();
    }

    private async Task NuevoAsync()
    {
        var editor = new ProveedorEditorViewModel();
        if (await DialogService.EditarProveedorAsync(editor) != true)
        {
            return;
        }

        try
        {
            await _repositorio.InsertarAsync(editor.ToModelo());
            await CargarAsync();
            MensajeEstado = "Proveedor registrado.";
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

        var editor = new ProveedorEditorViewModel(Seleccionado);
        if (await DialogService.EditarProveedorAsync(editor) != true)
        {
            return;
        }

        try
        {
            await _repositorio.ActualizarAsync(editor.ToModelo());
            await CargarAsync();
            MensajeEstado = "Proveedor actualizado.";
        }
        catch (Exception ex)
        {
            DialogService.Error(ex.Message);
        }
    }

    private async Task EliminarAsync()
    {
        if (Seleccionado is null || !DialogService.Confirmar($"¿Dar de baja el proveedor \"{Seleccionado.CompaniaNombre}\"? El registro no se borra, solo se marca como inactivo."))
        {
            return;
        }

        try
        {
            await _repositorio.EliminarAsync(Seleccionado.ProveedorID);
            await CargarAsync();
            MensajeEstado = "Proveedor dado de baja (eliminación lógica).";
        }
        catch (Exception ex)
        {
            DialogService.Error(ex.Message);
        }
    }

    private void Reemplazar(IEnumerable<Proveedor> items)
    {
        Proveedores.Clear();
        foreach (var proveedor in items)
        {
            Proveedores.Add(proveedor);
        }
    }
}
