using Infragistics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TradeStation.Infrastructure.CommonUtils;

namespace TradeStation.Modules.RealTimePrice.Converters
{
    /// <summary>
    /// Convert the selection number to the corresponding enum type.
    /// </summary>
    public sealed class RadioButtonToEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isMatch = false;
            int parameterIntValue;

            if (null != value
                && null != parameter 
                && int.TryParse(parameter.ToString(), out parameterIntValue))
            {
                int intValue = (int)value;

                if (intValue == parameterIntValue)
                {
                    isMatch = true;
                }
            }

            return isMatch;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int result = 0;
            int parameterIntValue;

            if (null != parameter
                && int.TryParse(parameter.ToString(), out parameterIntValue))
            {
                result = parameterIntValue;
            }

            return Enum.ToObject(targetType, result);
        }
    }
}
