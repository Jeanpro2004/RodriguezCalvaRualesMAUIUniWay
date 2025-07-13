using System.Globalization;

namespace RodriguezCalvaRualesMAUIUniWay.Converters
{
    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                var format = parameter?.ToString();
                if (!string.IsNullOrEmpty(format))
                    return string.Format(format, intValue);
                return intValue.ToString();
            }
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(value?.ToString(), out int result))
                return result;
            return 0;
        }
    }
}