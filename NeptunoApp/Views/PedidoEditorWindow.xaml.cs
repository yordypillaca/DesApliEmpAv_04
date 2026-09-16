using System.Windows;

namespace NeptunoApp.Views;

public partial class PedidoEditorWindow : Window
{
    public PedidoEditorWindow()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => EditorWindowBase.EnlazarCierre(this, DataContext);
    }
}
