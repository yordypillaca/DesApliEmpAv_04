using System.Windows;
using NeptunoApp.ViewModels;
using NeptunoApp.Views;

namespace NeptunoApp.Helpers;

/// <summary>
/// Abre ventanas de mantenimiento.
/// </summary>
public static class DialogService
{
    public static bool Confirmar(string mensaje, string titulo = "Confirmar")
    {
        return MessageBox.Show(mensaje, titulo, MessageBoxButton.YesNo, MessageBoxImage.Question)
               == MessageBoxResult.Yes;
    }

    public static void Aviso(string mensaje, string titulo = "Neptuno")
    {
        MessageBox.Show(mensaje, titulo, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public static void Error(string mensaje, string titulo = "Error")
    {
        MessageBox.Show(mensaje, titulo, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public static bool? EditarCategoria(CategoriaEditorViewModel vm)
        => Mostrar(new CategoriaEditorWindow { DataContext = vm });

    public static bool? EditarProducto(ProductoEditorViewModel vm)
        => Mostrar(new ProductoEditorWindow { DataContext = vm });

    public static bool? EditarProveedor(ProveedorEditorViewModel vm)
        => Mostrar(new ProveedorEditorWindow { DataContext = vm });

    public static bool? EditarPedido(PedidoEditorViewModel vm)
        => Mostrar(new PedidoEditorWindow { DataContext = vm });

    private static bool? Mostrar(Window ventana)
    {
        ventana.Owner = Application.Current.MainWindow;
        ventana.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        return ventana.ShowDialog();
    }
}
