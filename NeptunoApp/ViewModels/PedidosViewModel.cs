using System.Collections.ObjectModel;
using Neptuno.Data;
using Neptuno.Data.Models;
using NeptunoApp.Helpers;
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
        CargarCommand = new AsyncRelayCommand(_ => CargarAsync());
        NuevoCommand = new AsyncRelayCommand(_ => NuevoAsync());
        EditarCommand = new AsyncRelayCommand(_ => EditarAsync(), _ => Seleccionado is not null);
        EliminarCommand = new AsyncRelayCommand(_ => EliminarAsync(), _ => Seleccionado is not null);
        ConsultarReporteCommand = new AsyncRelayCommand(_ => ConsultarReporteAsync());
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

    public AsyncRelayCommand CargarCommand { get; }
    public AsyncRelayCommand NuevoCommand { get; }
    public AsyncRelayCommand EditarCommand { get; }
    public AsyncRelayCommand EliminarCommand { get; }
    public AsyncRelayCommand ConsultarReporteCommand { get; }

    public async Task CargarAsync()
    {
        try
        {
            var lista = await _repositorio.ListarAsync();
            Pedidos.Clear();
            foreach (var pedido in lista)
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

    public async Task ConsultarReporteAsync()
    {
        try
        {
            if (FechaInicio > FechaFin)
            {
                MensajeReporte = "La fecha de inicio no puede ser mayor que la fecha de fin.";
                return;
            }

            var lineas = await _repositorio.ListarDetallesPorFechasAsync(FechaInicio, FechaFin);
            Reporte.Clear();
            foreach (var linea in lineas)
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

    private async Task NuevoAsync()
    {
        var editor = new PedidoEditorViewModel();
        if (await DialogService.EditarPedidoAsync(editor) != true)
        {
            return;
        }

        try
        {
            await _repositorio.GuardarAsync(editor.ToModelo(), editor.Detalles.ToList());
            await CargarAsync();
            await ConsultarReporteAsync();
            MensajeEstado = "Pedido registrado.";
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

        var editor = new PedidoEditorViewModel(Seleccionado);
        if (await DialogService.EditarPedidoAsync(editor) != true)
        {
            return;
        }

        try
        {
            await _repositorio.GuardarAsync(editor.ToModelo(), editor.Detalles.ToList());
            await CargarAsync();
            await ConsultarReporteAsync();
            MensajeEstado = "Pedido actualizado.";
        }
        catch (Exception ex)
        {
            DialogService.Error(ex.Message);
        }
    }

    private async Task EliminarAsync()
    {
        if (Seleccionado is null || !DialogService.Confirmar($"¿Dar de baja el pedido #{Seleccionado.PedidoID}? El pedido no se borra, solo se marca como inactivo."))
        {
            return;
        }

        try
        {
            await _repositorio.EliminarAsync(Seleccionado.PedidoID);
            await CargarAsync();
            await ConsultarReporteAsync();
            MensajeEstado = "Pedido dado de baja (eliminación lógica).";
        }
        catch (Exception ex)
        {
            DialogService.Error(ex.Message);
        }
    }
}
