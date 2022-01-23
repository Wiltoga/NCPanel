using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NCPanel
{
    public class MultiplyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var left = value as double?;
            double right;
            if (left is not null && double.TryParse(parameter.ToString(), out right))
            {
                return left.Value * right;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var left = value as double?;
            double right;
            if (left is not null && double.TryParse(parameter.ToString(), out right))
            {
                return left.Value / right;
            }
            return 0;
        }
    }
}