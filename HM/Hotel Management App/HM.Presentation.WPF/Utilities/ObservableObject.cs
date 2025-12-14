using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HM.Presentation.WPF.Utilities;

/// <summary>
///     A base class implementing the <see cref="System.ComponentModel.INotifyPropertyChanged" /> interface
///     to simplify the creation of observable properties.
/// </summary>
/// <remarks>
///     This class provides a convenient way to notify the UI when property values change.
///     ViewModel classes can inherit from this class to easily implement property change notification.
/// </remarks>
public class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    ///     Notifies subscribers about a property change by invoking the <see cref="PropertyChanged" /> event.
    /// </summary>
    /// <param name="propertyName">The name of the property that has changed. Automatically populated by the compiler.</param>
    /// <remarks>
    ///     This method is typically used within property setters to notify listeners about changes in property values.
    ///     The optional parameter 'propertyName' is automatically inferred by the compiler if not explicitly provided.
    /// </remarks>
    public void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}