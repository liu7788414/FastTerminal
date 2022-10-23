using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TradeStation.Infrastructure.Converters
{
    public class PriceDigitalDisplayColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double numberValue = 0.0;
            if (value != null)
            {
                double.TryParse(value.ToString(), out numberValue);
            }

            if (numberValue > 0)
            {
                return Brushes.Red;
            }
            else if (numberValue < 0)
            {
                return Brushes.DarkGreen;
            }

            return Brushes.Black;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class NetValueDigitalDisplayColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double numberValue = 0.0;
            if (value != null)
            {
                double.TryParse(value.ToString(), out numberValue);
            }

            if (numberValue > 1)
            {
                return Brushes.Red;
            }
            else if (numberValue < 1)
            {
                return Brushes.DarkGreen;
            }

            return Brushes.Black;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class MarginPercentDisplayColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double numberValue = 0.0;
            if (value != null)
            {
                double.TryParse(value.ToString(), out numberValue);
            }

            if (numberValue < 20)
            {
                return Brushes.OrangeRed;
            }

            return Brushes.Black;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
