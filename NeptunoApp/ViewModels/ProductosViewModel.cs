using System.Collections.ObjectModel;
using NeptunoApp.Data;
using NeptunoApp.Helpers;
using NeptunoApp.Models;
using NeptunoApp.MVVM;

namespace NeptunoApp.ViewModels;

public class ProductosViewModel : ViewModelBase
{
    private readonly ProductoRepository _repositorio = new();
    private Producto? _seleccionado;
    private string _mensajeEstado = string.Empty;

    public ProductosViewModel()
    {
        CargarCommand = new RelayCommand(_ => Cargar());
        NuevoCommand = new RelayCommand(_ => Nuevo());
        EditarCommand = new RelayCommand(_ => Editar(), _ => Seleccionado is not null);
        EliminarCommand = new RelayCommand(_ => Eliminar(), _ => Seleccionado is not null);
        Cargar();
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

    public RelayCommand CargarCommand { get; }
    public RelayCommand NuevoCommand { get; }
    public RelayCommand EditarCommand { get; }
    public RelayCommand EliminarCommand { get; }

    public void Cargar()
    {
        try
        {
            Productos.Clear();
            foreach (var producto in _repositorio.Listar())
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

    private void Nuevo()
    {
        var editor = new ProductoEditorViewModel();
        if (DialogService.EditarProducto(editor) != true)
        {
            return;
        }

        try
        {
            _repositorio.Insertar(editor.ToModelo());
            Cargar();
            MensajeEstado = "Producto registrado.";
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

        var editor = new ProductoEditorViewModel(Seleccionado);
        if (DialogService.EditarProducto(editor) != true)
        {
            return;
        }

        try
        {
            _repositorio.Actualizar(editor.ToModelo());
            Cargar();
            MensajeEstado = "Producto actualizado.";
        }
        catch (Exception ex)
        {
            DialogService.Error(ex.Message);
        }
    }

    private void Eliminar()
    {
        if (Seleccionado is null || !DialogService.Confirmar($"¿Dar de baja el producto \"{Seleccionado.NombreProducto}\"? El registro no se borra, solo se marca como inactivo."))
        {
            return;
        }

        try
        {
            _repositorio.Eliminar(Seleccionado.ProductoID);
            Cargar();
            MensajeEstado = "Producto dado de baja (eliminación lógica).";
        }
        catch (Exception ex)
        {
            DialogService.Error(ex.Message);
        }
    }
}
