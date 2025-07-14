using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RodriguezCalvaRualesMAUIUniWay.API;

namespace RodriguezCalvaRualesMAUIUniWay.Converters
{
    public class EstadoToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Estado estado)
            {
                return estado == Estado.Pendiente || estado == Estado.Confirmada;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
