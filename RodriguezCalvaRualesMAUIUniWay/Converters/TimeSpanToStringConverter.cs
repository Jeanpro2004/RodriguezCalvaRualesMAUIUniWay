using System.Globalization;

namespace RodriguezCalvaRualesMAUIUniWay.Converters
{
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                var format = parameter?.ToString() ?? @"hh\:mm";
                return timeSpan.ToString(format);
            }
            return "00:00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (TimeSpan.TryParse(value?.ToString(), out TimeSpan result))
                return result;
            return TimeSpan.Zero;
        }
    }
}