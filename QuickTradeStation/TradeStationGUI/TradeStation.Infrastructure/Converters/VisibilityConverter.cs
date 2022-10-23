using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TradeStation.Infrastructure.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool visible = (bool)value;
            bool isRevert = false;
            if (parameter != null)
            {
                isRevert = parameter.ToString().Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
            }

            if (visible == false)
                if (isRevert == true)
                    return "Visible";
                else
                    return "Collapsed";
            else
                if (isRevert == true)
                    return "Collapsed";
                else
                    return "Visible";

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
