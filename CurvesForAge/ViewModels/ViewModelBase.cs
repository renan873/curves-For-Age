using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CurvesForAge.ViewModels;

public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected void SetValue<T>(ref T backingField, T value, Action? actionTrigger = null, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingField, value))
        {
            return;
        }
        backingField = value;
        OnPropertyChanged(propertyName);
        actionTrigger?.Invoke();
    }
}