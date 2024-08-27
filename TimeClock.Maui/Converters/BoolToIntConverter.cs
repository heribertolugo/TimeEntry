using System.Globalization;

namespace TimeClock.Maui.Converters
{
    internal sealed class BoolToIntConverter : IMarkupExtension, IValueConverter
    {
        public int TrueValue { get; set; } = 1;
        public int FalseValue { get; set; } = 0;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || !(value is bool val)) return null;

            return val ? TrueValue : FalseValue;
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
    internal sealed class BoolToDoubleConverter : IMarkupExtension, IValueConverter
    {
        public double TrueValue { get; set; } = 1;
        public double FalseValue { get; set; } = 0;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || !(value is bool val)) return null;

            return val ? TrueValue : FalseValue;
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
}
