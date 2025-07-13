using System.Globalization;

namespace RodriguezCalvaRualesMAUIUniWay.Converters
{
    public class DecimalToCurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
                return $"${decimalValue:F2}";
            if (value is double doubleValue)
                return $"${doubleValue:F2}";
            if (value is float floatValue)
                return $"${floatValue:F2}";
            return "$0.00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                var cleanValue = stringValue.Replace("$", "").Replace(",", "");
                if (decimal.TryParse(cleanValue, out decimal result))
                    return result;
            }
            return 0m;
        }
    }
}
