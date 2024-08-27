using System.Globalization;
using TimeClock.Maui.Models;
using TimeClock.Core.Models.EntityDtos;

namespace TimeClock.Maui.Converters
{
    internal class PunchTypeToDecimalConverter : IValueConverter
    {
        /// <summary>
        /// Returns a decimal number corresponding to the parameters passed based on PunchType being BarCode or not.
        /// </summary>
        /// <param name="value">A PunchType value</param>
        /// <param name="targetType">Not used</param>
        /// <param name="parameter">A PunchTypeToOpacityConverterParam object</param>
        /// <param name="culture">Not used</param>
        /// <returns>returns decimal value corresponding to the PunchType as specified by provided parameter</returns>
        /// <exception cref="ArgumentNullException">If value or parameter are null, a ArgumentNullException will be thrown</exception>
        /// <exception cref="ArgumentException">If value is not PunchType or parameter is not PunchTypeToOpacityConverterParam, a ArgumentException will be thrown.</exception>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return BindableProperty.UnsetValue;
            if (value is not PunchTypeDto)
                return BindableProperty.UnsetValue;
            if (parameter is not PunchTypeToDecimalConverterParam)
                return BindableProperty.UnsetValue;

            PunchTypeToDecimalConverterParam param = (PunchTypeToDecimalConverterParam)parameter;

            return ((PunchTypeDto)value) == PunchTypeDto.Barcode ? param.ValueIfBarCode : param.DefaultValue;
        }

        [Obsolete("Method has not been implemented", true)]
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
