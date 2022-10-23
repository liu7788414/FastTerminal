using Infragistics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TradeStation.Modules.RealTimePrice.Converters
{
    /// <summary>
    /// Convert the stock time to correct format for the axis.
    /// The input will be 00:00~04:00，the output will be 09:30~11:30 and 13:00~15:00.
    /// </summary>
    public sealed class StockDataVolumeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringFormatConverter = new StringFormatConverter();
            double stockVolume = 0;
            string result = "DEFAULT";

            if (null != value
                && value is double)
            {
                stockVolume = Math.Abs((double)value);

                if (stockVolume >= 1000000)
                {
                    stockVolume /= 1000000;
                    result = stockVolume + "M";
                }
                else if (stockVolume >= 1000)
                {
                    stockVolume /= 1000;
                    result = stockVolume + "K";
                }
                else
                {
                    result = stockVolume.ToString();
                }
            }

            return stringFormatConverter.Convert(result, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Do nothing.
            return value;
        }
    }
}
