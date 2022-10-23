using Infragistics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TradeStation.Infrastructure.Converters
{
    /// <summary>
    /// Convert the 2 double number to 1 int.
    /// If the first greater than the second binding, then return 1
    /// If the first less than the second binding, then return -1
    /// If the first equals the second binding, then return 0
    /// </summary>
    public sealed class IsGreaterThanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double firstDouble = 0;
            double secondDouble = 0;
            int result = 0;

            if (null != values[0] && values[0] is double
                && null != values[1] && values[1] is double)
            {
                firstDouble = (double)values[0];
                secondDouble = (double)values[1];
            }

            if (firstDouble > secondDouble)
            {
                result = 1;
            }
            else if (firstDouble < secondDouble)
            {
                result = -1;
            }

            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
