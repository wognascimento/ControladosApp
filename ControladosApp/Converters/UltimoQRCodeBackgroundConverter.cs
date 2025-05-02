using Microsoft.Maui.Controls;
using System;
using System.Globalization;

namespace ControladosApp.Converters;

public class UltimoQRCodeBackgroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isUltimo && isUltimo)
            return Color.FromArgb("#ccffcc"); // Verde clarinho
        else
            return Colors.Transparent; // Sem cor
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
