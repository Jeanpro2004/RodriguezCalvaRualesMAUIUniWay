using System.Collections;
using System.Globalization;

namespace RodriguezCalvaRualesMAUIUniWay.Converters
{
    public class ListCountToVisibilityConverter : IValueConverter
    {
        public bool InvertResult { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool hasItems = false;

            if (value is ICollection collection)
                hasItems = collection.Count > 0;
            else if (value is IEnumerable enumerable)
                hasItems = enumerable.Cast<object>().Any();
            else if (value is int count)
                hasItems = count > 0;

            return InvertResult ? !hasItems : hasItems;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
