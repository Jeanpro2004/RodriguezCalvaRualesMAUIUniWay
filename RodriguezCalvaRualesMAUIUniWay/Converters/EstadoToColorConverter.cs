using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RodriguezCalvaRualesMAUIUniWay.API;

namespace RodriguezCalvaRualesMAUIUniWay.Converters
{
    public class EstadoToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Estado estado)
            {
                return estado switch
                {
                    Estado.Pendiente => Color.FromArgb("#F39C12"),
                    Estado.Confirmada => Color.FromArgb("#27AE60"),
                    Estado.Cancelada => Color.FromArgb("#E74C3C"),
                    _ => Color.FromArgb("#95A5A6")
                };
            }
            return Color.FromArgb("#95A5A6");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
