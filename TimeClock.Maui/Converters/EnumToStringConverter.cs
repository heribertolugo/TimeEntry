using System.Globalization;
using System.Text;

namespace TimeClock.Maui.Converters;

/// <summary>
/// Converts enum to normalized text (space separated). Does not affect case.
/// </summary>
public class EnumToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return string.Empty;
        if (!value.GetType().IsEnum)
            return BindableProperty.UnsetValue;
        StringBuilder output = new();
        string? input = Enum.GetName(value.GetType(), value);

        if (input == null)
            return BindableProperty.UnsetValue;

        foreach(char kar in input)
        {
            if (char.IsUpper(kar))
                output.Append(' ');

            output.Append(kar);
        }

        return output.ToString().Trim();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
