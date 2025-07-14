using System.Globalization;

namespace RodriguezCalvaRualesMAUIUniWay.Converters
{
    public class BoolToUserTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? "🚗 Conductor Verificado" : "🚶 Pasajero Activo";
            return "👤 Usuario";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}