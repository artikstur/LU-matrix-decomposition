using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiApp1.ViewModel;

public class EntryViewModel: INotifyPropertyChanged
{
	private string _text;
    
	public string Text
    {
        get => _text;
		set
        {
            _text = value;
            OnPropertyChanged(nameof(value));
        }
	}

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        Console.WriteLine(_text);
    }
}