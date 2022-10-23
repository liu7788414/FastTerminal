using System;
using System.Globalization;
using System.Windows.Data;

namespace TradeStation.Infrastructure.Converters
{
    /// <summary>
    /// Reture the greater value in 2 double number.
    /// If parameter is true then return the less value.
    /// </summary>
    public sealed class GreaterValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            long firstValue = 0;
            long secondValue = 0;
            long result = 0;
            bool isRevert = false;

            if (null != parameter)
            {
                isRevert = parameter.ToString().Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
            }

            if (null != values[0] && values[0] is long
                && null != values[1] && values[1] is long)
            {
                firstValue = (long)values[0];
                secondValue = (long)values[1];
            }

            if (firstValue >= secondValue)
            {
                result = isRevert ? secondValue : firstValue;
            }
            else
            {
                result = isRevert ? firstValue : secondValue;
            }

            return result.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
