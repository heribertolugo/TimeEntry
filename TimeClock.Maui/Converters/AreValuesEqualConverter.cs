using System.Globalization;

namespace TimeClock.Maui.Converters;
public class AreValuesEqualConverter : IMultiValueConverter, IMarkupExtension
{
    public object TrueValue { get; set; } = true;
    public object FalseValue { get; set; } = false;
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        object? value = values.FirstOrDefault();

        foreach(object item in values)
        {
            if (!object.Equals(value, item))
                return this.FalseValue;
        }

        return this.TrueValue;
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    public object ProvideValue(IServiceProvider serviceProvider) => this;
}
