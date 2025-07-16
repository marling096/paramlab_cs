using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace components
{
    public class OriginalButtonBrushConverter : IMultiValueConverter
    {
        public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count < 2)
                return Brushes.LightGray;
            bool isCanny = values[0] is bool b1 && b1;
            bool isIsp = values[1] is bool b2 && b2;
            // 你可以根据 isCanny 和 isIsp 的组合自定义颜色逻辑
            return (isCanny || isIsp) ? Brushes.LightSkyBlue : Brushes.LightGray;
        }
    }
}
