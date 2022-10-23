using Infragistics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TradeStation.Infrastructure.Converters;

namespace TradeStation.Option.Converters
{
    public sealed class CombinedFuturePriceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            DigitFormatStringConverter digitConverter = new DigitFormatStringConverter();

            object[] combineFuturePriceValues = new object[2];
            double callOptionPrice = 0;
            double putOptionPrice = 0;
            double exercisePrice = 0;

            // 合成期货多头：买入CALL  卖出PUT      最终现金流  （期货价格-行权价）
            // 合成期货空头：买入PUT   卖出CALL     最终现金流  （行权价-期货价格）
            // 合成期货多头（买价）：行权价-PUT期权委托买价+CALL期权委托卖价
            // 合成期货空头（卖价）： -1*（PUT期权委托卖价-CALL期权委托买价-行权价）
            // 所以
            // 当显示买价时：callOptionPrice实为CALL期权委托卖价，putOptionPrice实为PUT期权委托买价
            // 对应的当显示卖价时：callOptionPrice实为CALL期权委托买价，putOptionPrice实为PUT期权委托卖价
            if (values.Count() == 4
                && null != values[0] && values[0] is double
                && null != values[1] && values[1] is double
                && null != values[2] && values[2] is double)
            {
                callOptionPrice = (double)values[0];
                putOptionPrice = (double)values[1];
                exercisePrice = (double)values[2];

                combineFuturePriceValues[0] = exercisePrice + callOptionPrice - putOptionPrice;
                combineFuturePriceValues[1] = values[3];
            }

            return digitConverter.Convert(combineFuturePriceValues, targetType, parameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
