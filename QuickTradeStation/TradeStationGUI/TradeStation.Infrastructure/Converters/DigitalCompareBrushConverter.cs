using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TradeStation.Infrastructure.Converters
{
    public class DigitalCompareBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType,
               object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                double value1;
                double value2;
                if (double.TryParse(values[0].ToString(), out value1) && double.TryParse(values[1].ToString(), out value2))
                {
                    if (value1 > value2)
                        return Brushes.Red;

                    else if (value1 < value2)
                        return Brushes.DarkGreen;
                }
                return Brushes.Black;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error convert number :" + ex.Message);
                return Brushes.Black;
            }

        }
        public object[] ConvertBack(object value, Type[] targetTypes,
               object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
