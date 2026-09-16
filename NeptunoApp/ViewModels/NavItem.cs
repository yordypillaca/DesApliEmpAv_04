using System.Collections.ObjectModel;

namespace NeptunoApp.ViewModels;

public class NavItem
{
    public string Titulo { get; init; } = string.Empty;
    public string Clave { get; init; } = string.Empty;
    public ObservableCollection<NavItem> Hijos { get; } = [];
}
