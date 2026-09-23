using System.Windows;
using NeptunoApp.ViewModels;

namespace NeptunoApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
        Loaded += MainWindow_OnLoaded;
    }

    // Evento async de punta a punta. No se usa .Result ni .Wait().
    private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            await vm.NavegarAsync("inicio");
        }
    }

    private async void MenuArbol_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (DataContext is MainViewModel vm && e.NewValue is NavItem item)
        {
            await vm.NavegarAsync(item.Clave);
        }
    }
}
