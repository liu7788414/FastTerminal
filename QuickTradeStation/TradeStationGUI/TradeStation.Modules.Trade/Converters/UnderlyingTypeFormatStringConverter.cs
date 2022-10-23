using System;
using System.Globalization;
using System.Windows.Data;

using TradeStation.Infrastructure.CommonUtils;

namespace TradeStation.Modules.Trade.Converters
{
    /// <summary>
    /// Convert the decimal digit number to string format.
    /// </summary>
    public sealed class UnderlyingTypeFormatStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue = 0;
            int digit = 2;

            if (null != values[0] && values[0] is double
                && null != values[1] && values[1] is eUnderlyingType)
            {
                eUnderlyingType underlyingType = (eUnderlyingType)values[1];

                doubleValue = (double)values[0];
                if (underlyingType == eUnderlyingType.ETF)
                {
                    digit = 3;
                }
            }

            return string.Format("{0:N" + digit + "}", doubleValue);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
