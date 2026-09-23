using Neptuno.Data.Models;
using NeptunoApp.MVVM;

namespace NeptunoApp.ViewModels;

public class ProveedorEditorViewModel : ViewModelBase
{
    private string _companiaNombre = string.Empty;
    private string? _nombreContacto;
    private string? _cargoContacto;
    private string? _direccion;
    private string? _ciudad;
    private string? _codigoPostal;
    private string? _pais;
    private string? _telefono;
    private string? _fax;
    private string _mensajeEstado = string.Empty;

    public ProveedorEditorViewModel(Proveedor? proveedor = null)
    {
        EsNuevo = proveedor is null;
        Titulo = EsNuevo ? "Nuevo proveedor" : "Editar proveedor";
        ProveedorID = proveedor?.ProveedorID ?? 0;
        CompaniaNombre = proveedor?.CompaniaNombre ?? string.Empty;
        NombreContacto = proveedor?.NombreContacto;
        CargoContacto = proveedor?.CargoContacto;
        Direccion = proveedor?.Direccion;
        Ciudad = proveedor?.Ciudad;
        CodigoPostal = proveedor?.CodigoPostal;
        Pais = proveedor?.Pais;
        Telefono = proveedor?.Telefono;
        Fax = proveedor?.Fax;

        AceptarCommand = new RelayCommand(_ => Aceptar());
        CancelarCommand = new RelayCommand(_ => Cancelar());
    }

    public event Action<bool>? SolicitarCierre;

    public bool EsNuevo { get; }
    public string Titulo { get; }
    public int ProveedorID { get; }

    public string CompaniaNombre { get => _companiaNombre; set => SetField(ref _companiaNombre, value); }
    public string? NombreContacto { get => _nombreContacto; set => SetField(ref _nombreContacto, value); }
    public string? CargoContacto { get => _cargoContacto; set => SetField(ref _cargoContacto, value); }
    public string? Direccion { get => _direccion; set => SetField(ref _direccion, value); }
    public string? Ciudad { get => _ciudad; set => SetField(ref _ciudad, value); }
    public string? CodigoPostal { get => _codigoPostal; set => SetField(ref _codigoPostal, value); }
    public string? Pais { get => _pais; set => SetField(ref _pais, value); }
    public string? Telefono { get => _telefono; set => SetField(ref _telefono, value); }
    public string? Fax { get => _fax; set => SetField(ref _fax, value); }

    public string MensajeEstado
    {
        get => _mensajeEstado;
        private set => SetField(ref _mensajeEstado, value);
    }

    public RelayCommand AceptarCommand { get; }
    public RelayCommand CancelarCommand { get; }

    public Proveedor ToModelo()
    {
        return new Proveedor
        {
            ProveedorID = ProveedorID,
            CompaniaNombre = CompaniaNombre.Trim(),
            NombreContacto = Texto(NombreContacto),
            CargoContacto = Texto(CargoContacto),
            Direccion = Texto(Direccion),
            Ciudad = Texto(Ciudad),
            CodigoPostal = Texto(CodigoPostal),
            Pais = Texto(Pais),
            Telefono = Texto(Telefono),
            Fax = Texto(Fax)
        };
    }

    private static string? Texto(string? valor)
        => string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

    private void Aceptar()
    {
        if (string.IsNullOrWhiteSpace(CompaniaNombre))
        {
            MensajeEstado = "El nombre de la compañía es obligatorio.";
            return;
        }

        SolicitarCierre?.Invoke(true);
    }

    private void Cancelar() => SolicitarCierre?.Invoke(false);
}
