using Infragistics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TradeStation.Infrastructure.CommonUtils;
using TradeStation.Infrastructure.Converters;

namespace TradeStation.Modules.RealTimePrice.Converters
{
    /// <summary>
    /// Convert the decimal digit number to string format.
    /// </summary>
    public sealed class QuotationPriceDigitFormatStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            DigitFormatStringConverter digitConverter = new DigitFormatStringConverter();

            eCategory securityType = eCategory.股票;

            if (values.Count() >= 3 && values[2] is eCategory)
            {
                securityType = (eCategory)values[2];

                if (securityType == eCategory.期货
                    || securityType == eCategory.期权)
                {
                    return "-";
                }
            }

            return digitConverter.Convert(values, targetType, parameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
