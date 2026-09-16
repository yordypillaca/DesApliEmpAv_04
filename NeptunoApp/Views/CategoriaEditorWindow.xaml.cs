using System.Windows;

namespace NeptunoApp.Views;

public partial class CategoriaEditorWindow : Window
{
    public CategoriaEditorWindow()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => EditorWindowBase.EnlazarCierre(this, DataContext);
    }
}
