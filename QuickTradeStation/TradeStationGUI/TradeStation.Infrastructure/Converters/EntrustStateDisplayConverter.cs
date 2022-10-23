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
    public class EntrustStateDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                switch ((eEntrustState)value)
                {
                    case eEntrustState.已成:
                    case eEntrustState.部成:
                        {
                            return Brushes.Green;
                        }       
                    case eEntrustState.已报:
                    case eEntrustState.正报:
                    case eEntrustState.待撤:
                    case eEntrustState.未报:
                        {
                            return Brushes.Black;
                        }
                    case eEntrustState.部撤:
                    case eEntrustState.已撤:
                    case eEntrustState.废单:
                        {
                            return Brushes.DarkGray;
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


    public class SecurityStatusDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                switch ((SecurityStatus) value)
                {
                    case SecurityStatus.停牌:
                    {
                        return Brushes.Brown;
                    }
                    case SecurityStatus.退市:
                    {
                        return Brushes.Red;
                    }
                }
            }

            return Brushes.Green;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }


    public class ArbitrageStatusDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                switch ((ArbitrageStatus)value)
                {
                    case ArbitrageStatus.停止:
                        {
                            return Brushes.Gray;
                        }
                    case ArbitrageStatus.已执行:
                        {
                            return Brushes.Green;
                        }
                    case ArbitrageStatus.正在执行:
                        {
                            return Brushes.Orange;
                        }
                    case ArbitrageStatus.正在监控:
                        {
                            return Brushes.Red;
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
}
