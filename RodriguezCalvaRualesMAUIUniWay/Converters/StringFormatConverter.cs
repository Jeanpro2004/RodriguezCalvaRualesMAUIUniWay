using System.Globalization;

namespace RodriguezCalvaRualesMAUIUniWay.Converters
{
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            var format = parameter?.ToString();
            if (string.IsNullOrEmpty(format))
                return value.ToString();

            try
            {
                return string.Format(format, value);
            }
            catch
            {
                return value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
