using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace components
{
    public class BoolToBrushConverter : IValueConverter
    {
        public IBrush TrueBrush { get; set; } = Brushes.LightSkyBlue;
        public IBrush FalseBrush { get; set; } = Brushes.LightGray;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is bool b && b ? TrueBrush : FalseBrush;

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
