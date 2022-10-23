using System;
using System.Globalization;
using System.Windows.Data;

using Infragistics;

using TradeStation.Infrastructure.Helpers;
using TradeStation.Infrastructure.Models;

namespace TradeStation.Modules.RealTimePrice.Converters
{
    /// <summary>
    /// Convert the stock time to correct format for the axis.
    /// The input will be 00:00~04:00，the output will be 09:30~11:30 and 13:00~15:00.
    /// </summary>
    public sealed class StockTimeConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringFormatConverter = new StringFormatConverter();
            DateTime stockTime = DateTime.MinValue;

            if (value.Length == 3
                && null != value[0] && value[0] is DateTime
                && null != value[1] && value[1] is string
                && null != value[2] && value[1] is string)
            {
                stockTime = (DateTime)value[0];
                var exSecID = new ExSecID((string)value[1], (string)value[2]);

                stockTime = RealTimePriceDateTimeConvertHelper.Instance.ConvertToActualTime(stockTime, exSecID);
            }

            return stringFormatConverter.Convert(stockTime, targetType, parameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
