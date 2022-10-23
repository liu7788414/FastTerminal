using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using TradeStation.Infrastructure.CommonUtils;

namespace TradeStation.Infrastructure.Converters
{
    public class StockQuantityDisplayConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double qty = 0.0;
            eCategory securityType = eCategory.股票;
            int result = 0;

            if (null != values[0]
                && null != values[1] && values[1] is eCategory)
            {
                double.TryParse(values[0].ToString(), out qty);
                securityType = (eCategory)values[1];

                // 股票和基金是以100的单位数量为1手
                if (securityType == eCategory.股票 || securityType == eCategory.基金)
                {
                    result = (int)(qty / 100);
                }
                else
                {
                    result = (int)qty;
                }
            }

            return result.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
