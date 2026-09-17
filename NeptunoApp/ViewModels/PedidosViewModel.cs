using System.Collections.ObjectModel;
using NeptunoApp.Data;
using NeptunoApp.Helpers;
using NeptunoApp.Models;
using NeptunoApp.MVVM;

namespace NeptunoApp.ViewModels;

public class PedidosViewModel : ViewModelBase
{
    private readonly PedidoRepository _repositorio = new();
    private Pedido? _seleccionado;
    private DateTime _fechaInicio = new(2026, 8, 1);
    private DateTime _fechaFin = new(2026, 8, 31);
    private string _mensajeEstado = string.Empty;
    private string _mensajeReporte = string.Empty;
    private int _tabSeleccionado;

    public PedidosViewModel()
    {
        CargarCommand = new RelayCommand(_ => Cargar());
        NuevoCommand = new RelayCommand(_ => Nuevo());
        EditarCommand = new RelayCommand(_ => Editar(), _ => Seleccionado is not null);
        EliminarCommand = new RelayCommand(_ => Eliminar(), _ => Seleccionado is not null);
        ConsultarReporteCommand = new RelayCommand(_ => ConsultarReporte());
        Cargar();
        ConsultarReporte();
    }

    public ObservableCollection<Pedido> Pedidos { get; } = [];
    public ObservableCollection<DetallePedido> Reporte { get; } = [];

    public Pedido? Seleccionado
    {
        get => _seleccionado;
        set => SetField(ref _seleccionado, value);
    }

    public DateTime FechaInicio
    {
        get => _fechaInicio;
        set => SetField(ref _fechaInicio, value);
    }

    public DateTime FechaFin
    {
        get => _fechaFin;
        set => SetField(ref _fechaFin, value);
    }

    public string MensajeEstado
    {
        get => _mensajeEstado;
        private set => SetField(ref _mensajeEstado, value);
    }

    public string MensajeReporte
    {
        get => _mensajeReporte;
        private set => SetField(ref _mensajeReporte, value);
    }

    public int TabSeleccionado
    {
        get => _tabSeleccionado;
        set => SetField(ref _tabSeleccionado, value);
    }

    public decimal TotalReporte => Reporte.Sum(d => d.Importe);

    public RelayCommand CargarCommand { get; }
    public RelayCommand NuevoCommand { get; }
    public RelayCommand EditarCommand { get; }
    public RelayCommand EliminarCommand { get; }
    public RelayCommand ConsultarReporteCommand { get; }

    public void Cargar()
    {
        try
        {
            Pedidos.Clear();
            foreach (var pedido in _repositorio.Listar())
            {
                Pedidos.Add(pedido);
            }

            MensajeEstado = $"{Pedidos.Count} pedido(s) cargado(s).";
        }
        catch (Exception ex)
        {
            MensajeEstado = $"Error al cargar: {ex.Message}";
        }
    }

    public void ConsultarReporte()
    {
        try
        {
            if (FechaInicio > FechaFin)
            {
                MensajeReporte = "La fecha de inicio no puede ser mayor que la fecha de fin.";
                return;
            }

            Reporte.Clear();
            foreach (var linea in _repositorio.ListarDetallesPorFechas(FechaInicio, FechaFin))
            {
                Reporte.Add(linea);
            }

            OnPropertyChanged(nameof(TotalReporte));
            MensajeReporte = $"{Reporte.Count} línea(s) entre {FechaInicio:dd/MM/yyyy} y {FechaFin:dd/MM/yyyy}. Total: {TotalReporte:N2}";
        }
        catch (Exception ex)
        {
            MensajeReporte = $"Error al consultar el reporte: {ex.Message}";
        }
    }

    private void Nuevo()
    {
        var editor = new PedidoEditorViewModel();
        if (DialogService.EditarPedido(editor) != true)
        {
            return;
        }

        try
        {
            _repositorio.Guardar(editor.ToModelo(), editor.Detalles.ToList());
            Cargar();
            ConsultarReporte();
            MensajeEstado = "Pedido registrado.";
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

        var editor = new PedidoEditorViewModel(Seleccionado);
        if (DialogService.EditarPedido(editor) != true)
        {
            return;
        }

        try
        {
            _repositorio.Guardar(editor.ToModelo(), editor.Detalles.ToList());
            Cargar();
            ConsultarReporte();
            MensajeEstado = "Pedido actualizado.";
        }
        catch (Exception ex)
        {
            DialogService.Error(ex.Message);
        }
    }

    private void Eliminar()
    {
        if (Seleccionado is null || !DialogService.Confirmar($"¿Dar de baja el pedido #{Seleccionado.PedidoID}? El pedido no se borra, solo se marca como inactivo."))
        {
            return;
        }

        try
        {
            _repositorio.Eliminar(Seleccionado.PedidoID);
            Cargar();
            ConsultarReporte();
            MensajeEstado = "Pedido dado de baja (eliminación lógica).";
        }
        catch (Exception ex)
        {
            DialogService.Error(ex.Message);
        }
    }
}
