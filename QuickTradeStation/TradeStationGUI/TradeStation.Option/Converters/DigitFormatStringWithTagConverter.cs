using System;
using System.Globalization;
using System.Windows.Data;

using TradeStation.Infrastructure.Converters;

namespace TradeStation.Option.Converters
{
    /// <summary>
    /// Convert the decimal digit number to string format.
    /// </summary>
    public sealed class DigitFormatStringWithTagConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var digitFormatStringCvt = new DigitFormatStringConverter();

            string additionTag = string.Empty;

            if (null != values[2] && values[2] is string)
            {
                additionTag = values[2].ToString();
            }

            return digitFormatStringCvt.Convert(values, targetType, parameter, culture).ToString() + additionTag;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
