using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RodriguezCalvaRualesMAUIUniWay.Converters
{
    public class EstadoToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string estado)
            {
                return estado switch
                {
                    "Pendiente" => Color.FromArgb("#F39C12"),
                    "Confirmada" => Color.FromArgb("#27AE60"),
                    "Cancelada" => Color.FromArgb("#E74C3C"),
                    "Completada" => Color.FromArgb("#3498DB"),
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
