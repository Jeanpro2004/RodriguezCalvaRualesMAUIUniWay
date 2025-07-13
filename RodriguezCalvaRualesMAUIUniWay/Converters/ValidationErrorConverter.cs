using System.Globalization;

namespace RodriguezCalvaRualesMAUIUniWay.Converters
{
    public class ValidationErrorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string error && !string.IsNullOrEmpty(error))
                return true; // Show error
            return false; // Hide error
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}