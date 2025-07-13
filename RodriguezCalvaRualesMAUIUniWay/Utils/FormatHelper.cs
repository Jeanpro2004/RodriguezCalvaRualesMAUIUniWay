namespace RodriguezCalvaRualesMAUIUniWay.Utils
{
    public static class FormatHelper
    {
        public static string FormatCurrency(decimal amount)
        {
            return $"${amount:F2}";
        }

        public static string FormatDistance(double kilometers)
        {
            if (kilometers < 1)
                return $"{(int)(kilometers * 1000)} m";

            return $"{kilometers:F1} km";
        }

        public static string FormatDuration(int minutes)
        {
            if (minutes < 60)
                return $"{minutes} min";

            var hours = minutes / 60;
            var remainingMinutes = minutes % 60;

            if (remainingMinutes == 0)
                return $"{hours} h";

            return $"{hours} h {remainingMinutes} min";
        }

        public static string FormatPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return string.Empty;

            // Remove formatting
            var cleaned = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

            // Add Ecuador prefix if not present
            if (!cleaned.StartsWith("+593") && !cleaned.StartsWith("593"))
                cleaned = "+593" + cleaned;
            else if (cleaned.StartsWith("593"))
                cleaned = "+" + cleaned;

            // Format as +593 XX XXX XXXX
            if (cleaned.Length == 13) // +593xxxxxxxxx
            {
                return $"{cleaned.Substring(0, 4)} {cleaned.Substring(4, 2)} {cleaned.Substring(6, 3)} {cleaned.Substring(9)}";
            }

            return cleaned;
        }

        public static string FormatPlate(string plate)
        {
            if (string.IsNullOrWhiteSpace(plate))
                return string.Empty;

            return plate.ToUpper();
        }

        public static string FormatCapacity(int current, int total)
        {
            return $"{current}/{total}";
        }

        public static string FormatPercentage(double value)
        {
            return $"{value:P0}";
        }

        public static string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            if (text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength - 3) + "...";
        }

        public static string FormatIdBanner(string idBanner)
        {
            if (string.IsNullOrWhiteSpace(idBanner))
                return string.Empty;

            return idBanner.ToUpper();
        }

        public static string FormatRating(double rating)
        {
            return $"⭐ {rating:F1}";
        }

        public static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }
    }
}