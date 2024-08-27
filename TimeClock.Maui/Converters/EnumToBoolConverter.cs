using System.Globalization;

namespace TimeClock.Maui.Converters
{
    public class EnumToBoolConverter : IMarkupExtension, IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (parameter == null) return BindableProperty.UnsetValue;

            return parameter.Equals(value);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return BindableProperty.UnsetValue;

            return ((bool)value) == true ? parameter : BindableProperty.UnsetValue;
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
