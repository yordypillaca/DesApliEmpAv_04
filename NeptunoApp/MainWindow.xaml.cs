using System.Windows;
using NeptunoApp.ViewModels;

namespace NeptunoApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }

    private void MenuArbol_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (DataContext is MainViewModel vm && e.NewValue is NavItem item)
        {
            vm.Navegar(item.Clave);
        }
    }
}
