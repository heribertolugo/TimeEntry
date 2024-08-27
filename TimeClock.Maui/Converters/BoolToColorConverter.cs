using System.Globalization;

namespace TimeClock.Maui.Converters;

internal sealed class BoolToColorConverter : IMarkupExtension, IValueConverter
{
    public Color TrueValue { get; set; } = Colors.Transparent;
    public Color FalseValue { get; set; } = Colors.Red;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || !(value is bool val)) return this.FalseValue;

        return val ? this.TrueValue : this.FalseValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}
