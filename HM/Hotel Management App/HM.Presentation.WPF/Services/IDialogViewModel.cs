namespace HM.Presentation.WPF.Services;

//TODO Add summary
public interface IDialogViewModel
{
    /// <summary>
    /// The ViewModel triggers this event when it wants to close.
    /// The bool? is the "DialogResult" (true = saved, false = cancelled).
    /// </summary>
    event Action<bool?> RequestClose;
}