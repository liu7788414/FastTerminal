using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TradeStation.Infrastructure.Converters
{
    public class StringNullOrEmptyReplacementConverter : IValueConverter
    {
        // If value is null or empty string, replace the value with parameter string.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (null == value || string.IsNullOrEmpty(value.ToString())) ? parameter.ToString() : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
