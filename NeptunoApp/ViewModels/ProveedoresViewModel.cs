using System.Collections.ObjectModel;
using NeptunoApp.Data;
using NeptunoApp.Helpers;
using NeptunoApp.Models;
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
        CargarCommand = new RelayCommand(_ => Cargar());
        BuscarCommand = new RelayCommand(_ => Buscar());
        LimpiarCommand = new RelayCommand(_ => Limpiar());
        NuevoCommand = new RelayCommand(_ => Nuevo());
        EditarCommand = new RelayCommand(_ => Editar(), _ => Seleccionado is not null);
        EliminarCommand = new RelayCommand(_ => Eliminar(), _ => Seleccionado is not null);
        Cargar();
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

    public RelayCommand CargarCommand { get; }
    public RelayCommand BuscarCommand { get; }
    public RelayCommand LimpiarCommand { get; }
    public RelayCommand NuevoCommand { get; }
    public RelayCommand EditarCommand { get; }
    public RelayCommand EliminarCommand { get; }

    public void Cargar()
    {
        try
        {
            Reemplazar(_repositorio.Listar());
            MensajeEstado = $"{Proveedores.Count} proveedor(es) cargado(s).";
        }
        catch (Exception ex)
        {
            MensajeEstado = $"Error al cargar: {ex.Message}";
        }
    }

    private void Buscar()
    {
        try
        {
            Reemplazar(_repositorio.Buscar(FiltroContacto, FiltroCiudad));
            MensajeEstado = $"{Proveedores.Count} proveedor(es) encontrados con los filtros.";
        }
        catch (Exception ex)
        {
            MensajeEstado = $"Error al buscar: {ex.Message}";
        }
    }

    private void Limpiar()
    {
        FiltroContacto = string.Empty;
        FiltroCiudad = string.Empty;
        Cargar();
    }

    private void Nuevo()
    {
        var editor = new ProveedorEditorViewModel();
        if (DialogService.EditarProveedor(editor) != true)
        {
            return;
        }

        try
        {
            _repositorio.Insertar(editor.ToModelo());
            Cargar();
            MensajeEstado = "Proveedor registrado.";
        }
        catch (Exception ex)
        {
            DialogService.Error(ex.Message);
        }
    }

    private void Editar()
    {
        if (Seleccionado is null)
        {
            return;
        }

        var editor = new ProveedorEditorViewModel(Seleccionado);
        if (DialogService.EditarProveedor(editor) != true)
        {
            return;
        }

        try
        {
            _repositorio.Actualizar(editor.ToModelo());
            Cargar();
            MensajeEstado = "Proveedor actualizado.";
        }
        catch (Exception ex)
        {
            DialogService.Error(ex.Message);
        }
    }

    private void Eliminar()
    {
        if (Seleccionado is null || !DialogService.Confirmar($"¿Dar de baja el proveedor \"{Seleccionado.CompaniaNombre}\"? El registro no se borra, solo se marca como inactivo."))
        {
            return;
        }

        try
        {
            _repositorio.Eliminar(Seleccionado.ProveedorID);
            Cargar();
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
