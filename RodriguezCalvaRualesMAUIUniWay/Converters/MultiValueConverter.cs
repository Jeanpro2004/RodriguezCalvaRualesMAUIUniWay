using System.Globalization;

namespace RodriguezCalvaRualesMAUIUniWay.Converters
{
    public class MultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0)
                return string.Empty;

            var format = parameter?.ToString() ?? "{0}";

            try
            {
                return string.Format(format, values);
            }
            catch
            {
                return string.Join(" ", values.Where(v => v != null));
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}