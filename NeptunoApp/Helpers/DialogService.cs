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

    public static void Error(string mensaje, string titulo = "Error")
    {
        MessageBox.Show(mensaje, titulo, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public static Task<bool?> EditarCategoriaAsync(CategoriaEditorViewModel vm)
        => Task.FromResult(Mostrar(new CategoriaEditorWindow { DataContext = vm }));

    public static async Task<bool?> EditarProductoAsync(ProductoEditorViewModel vm)
    {
        await vm.CargarCombosAsync();
        return Mostrar(new ProductoEditorWindow { DataContext = vm });
    }

    public static Task<bool?> EditarProveedorAsync(ProveedorEditorViewModel vm)
        => Task.FromResult(Mostrar(new ProveedorEditorWindow { DataContext = vm }));

    public static async Task<bool?> EditarPedidoAsync(PedidoEditorViewModel vm)
    {
        await vm.CargarCombosAsync();
        return Mostrar(new PedidoEditorWindow { DataContext = vm });
    }

    private static bool? Mostrar(Window ventana)
    {
        ventana.Owner = Application.Current.MainWindow;
        ventana.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        return ventana.ShowDialog();
    }
}
