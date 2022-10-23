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
    public class EntrustDirectionDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is eEntrustDirection)
            {
                switch ((eEntrustDirection)value)
                {
                    case eEntrustDirection.买入:
                    case eEntrustDirection.融资回购:
                    case eEntrustDirection.基金合并:
                        {
                            return Brushes.Red;
                        }
                    case eEntrustDirection.卖出:
                    case eEntrustDirection.融券回购:
                    case eEntrustDirection.基金分拆:
                        {
                            return Brushes.Green;
                        }
                }
            }

            return parameter;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }


    public class FuturesDirectionDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                switch ((eFuturesDirection)value)
                {
                    case eFuturesDirection.开仓:
                        {
                            return Brushes.Red;
                        }
                    case eFuturesDirection.平仓:
                        {
                            return Brushes.Green;
                        }
                }
            }

            return Brushes.Red;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class PositionFlagDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                switch ((ePositionFlag)value)
                {
                    case ePositionFlag.多头持仓:
                        {
                            return Brushes.Red;
                        }
                    case ePositionFlag.空头持仓:
                        {
                            return Brushes.Green;
                        }
                }
            }

            return Brushes.Red;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class OptionTypeDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                switch ((eOptionType)value)
                {
                    case eOptionType.认购期权:
                        {
                            return Brushes.Red;
                        }
                    case eOptionType.认沽期权:
                        {
                            return Brushes.Green;
                        }
                }
            }

            return Brushes.Red;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class RatioDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is string)
            {
                var s = (string)value;

                if (s.Contains("%"))
                {
                    s = s.Replace("%", "");

                    var v = System.Convert.ToDouble(s)/100;

                    return v;
                }
            }

            return parameter;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is double)
            {
                var d = (double) value;

                d = d*100;

                return System.Convert.ToString(d) + "%";
            }

            return null;
        }
    }
}
