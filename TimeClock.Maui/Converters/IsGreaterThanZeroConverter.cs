using System.Globalization;

namespace TimeClock.Maui.Converters;

public class IsGreaterThanZeroConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        int valueAsInt = -1;
        if (int.TryParse(value?.ToString(), out valueAsInt))
            return valueAsInt > 0;

        if (value is IEnumerable<object> objects)
            return objects.Any();
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
