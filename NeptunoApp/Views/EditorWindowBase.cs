using System.Windows;
using NeptunoApp.ViewModels;

namespace NeptunoApp.Views;

internal static class EditorWindowBase
{
    public static void EnlazarCierre(Window ventana, object? dataContext)
    {
        if (dataContext is CategoriaEditorViewModel categoria)
        {
            categoria.SolicitarCierre += aceptado => Cerrar(ventana, aceptado);
        }
        else if (dataContext is ProductoEditorViewModel producto)
        {
            producto.SolicitarCierre += aceptado => Cerrar(ventana, aceptado);
        }
        else if (dataContext is ProveedorEditorViewModel proveedor)
        {
            proveedor.SolicitarCierre += aceptado => Cerrar(ventana, aceptado);
        }
        else if (dataContext is PedidoEditorViewModel pedido)
        {
            pedido.SolicitarCierre += aceptado => Cerrar(ventana, aceptado);
        }
    }

    private static void Cerrar(Window ventana, bool aceptado)
    {
        ventana.DialogResult = aceptado;
    }
}
