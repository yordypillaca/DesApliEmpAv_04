using System.Windows;

namespace NeptunoApp.Views;

public partial class ProveedorEditorWindow : Window
{
    public ProveedorEditorWindow()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => EditorWindowBase.EnlazarCierre(this, DataContext);
    }
}
