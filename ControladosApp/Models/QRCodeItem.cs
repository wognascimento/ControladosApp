
using System.ComponentModel;

namespace ControladosApp.Models;

public class QRCodeItem : INotifyPropertyChanged
{
    private bool _isUltimo;

    public string Barcode { get; set; } = string.Empty; // Fix: Initialize the non-nullable property

    public bool IsUltimo
    {
        get => _isUltimo;
        set
        {
            if (_isUltimo != value)
            {
                _isUltimo = value;
                OnPropertyChanged(nameof(IsUltimo));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
