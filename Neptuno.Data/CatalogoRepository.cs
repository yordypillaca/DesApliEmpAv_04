using System.Data;
using Neptuno.Data.Models;

namespace Neptuno.Data;

/// <summary>
/// Catálogos auxiliares en modo DESCONECTADO (DataSet).
/// </summary>
public class CatalogoRepository
{
    public async Task<List<Cliente>> ListarClientesAsync()
    {
        var dataSet = await DisconnectedHelper.FillAsync("usp_Cliente_Listar");
        return DisconnectedHelper.Rows(DisconnectedHelper.FirstTable(dataSet)).Select(row => new Cliente
        {
            ClienteID = row.GetInt32("ClienteID"),
            Empresa = row.GetString("Empresa"),
            NombreContacto = row.GetNullableString("NombreContacto"),
            Ciudad = row.GetNullableString("Ciudad"),
            Pais = row.GetNullableString("Pais"),
            Telefono = row.GetNullableString("Telefono")
        }).ToList();
    }

    public async Task<List<Empleado>> ListarEmpleadosAsync()
    {
        var dataSet = await DisconnectedHelper.FillAsync("usp_Empleado_Listar");
        return DisconnectedHelper.Rows(DisconnectedHelper.FirstTable(dataSet)).Select(row => new Empleado
        {
            EmpleadoID = row.GetInt32("EmpleadoID"),
            Nombre = row.GetString("Nombre"),
            Apellidos = row.GetString("Apellidos"),
            Cargo = row.GetNullableString("Cargo"),
            Ciudad = row.GetNullableString("Ciudad"),
            Pais = row.GetNullableString("Pais"),
            NombreCompleto = row.Table.Columns.Contains("NombreCompleto")
                ? row.GetString("NombreCompleto")
                : $"{row.GetString("Nombre")} {row.GetString("Apellidos")}"
        }).ToList();
    }

    public async Task<List<Transportista>> ListarTransportistasAsync()
    {
        var dataSet = await DisconnectedHelper.FillAsync("usp_Transportista_Listar");
        return DisconnectedHelper.Rows(DisconnectedHelper.FirstTable(dataSet)).Select(row => new Transportista
        {
            TransportistaID = row.GetInt32("TransportistaID"),
            CompaniaNombre = row.GetString("CompaniaNombre"),
            Telefono = row.GetNullableString("Telefono")
        }).ToList();
    }
}
