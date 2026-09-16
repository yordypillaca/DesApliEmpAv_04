using System.Windows;

namespace NeptunoApp.Views;

public partial class ProductoEditorWindow : Window
{
    public ProductoEditorWindow()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => EditorWindowBase.EnlazarCierre(this, DataContext);
    }
}
