using System.ComponentModel;
using QuitSmoke.Services;

namespace QuitSmoke.ViewModels;

/// <summary>
/// Base de las pantallas: textos por clave en el idioma activo y un aviso de «ha cambiado todo» que
/// repinta las etiquetas enlazadas. La logica vive aqui para poder probarla sin pantalla (General 8.6).
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    protected ViewModelBase(ILocalizationService loc) => Loc = loc;

    protected ILocalizationService Loc { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected string L(string key) => Loc.GetString(key);

    /// <summary>Cadena vacia = han cambiado todas las propiedades.</summary>
    public void RaiseAllChanged() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
}
