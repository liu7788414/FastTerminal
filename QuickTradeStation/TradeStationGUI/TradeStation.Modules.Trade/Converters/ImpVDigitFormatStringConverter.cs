using System;
using System.Globalization;
using System.Windows.Data;

using TradeStation.Infrastructure.Helpers;

namespace TradeStation.Modules.Trade.Converters
{
    /// <summary>
    /// Convert the decimal digit number to string format.
    /// </summary>
    public sealed class ImpVDigitFormatStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue = 0;
            int digit = 2;

            if (null != values[0] && values[0] is double
                && null != values[1] && values[1] is int)
            {
                doubleValue = (double)values[0];
                digit = (int)values[1];
            }

            if (doubleValue == OptionFinanceCalculator.ERROR)
            {
                return "--";
            }
            else
            {
                doubleValue *= 100;
            }

            return string.Format("{0:F" + digit + "}", doubleValue);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
