using System.Globalization;

namespace RodriguezCalvaRualesMAUIUniWay.Utils
{
    public static class DateTimeHelper
    {
        private static readonly CultureInfo EcuadorCulture = new("es-EC");

        public static string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm", EcuadorCulture);
        }

        public static string FormatDate(DateTime date)
        {
            return date.ToString("dd/MM/yyyy", EcuadorCulture);
        }

        public static string FormatTime(TimeSpan time)
        {
            return time.ToString(@"hh\:mm");
        }

        public static string FormatTime(DateTime dateTime)
        {
            return dateTime.ToString("HH:mm", EcuadorCulture);
        }

        public static string GetRelativeTime(DateTime dateTime)
        {
            var diff = DateTime.Now - dateTime;

            if (diff.TotalSeconds < 60)
                return "Hace un momento";

            if (diff.TotalMinutes < 60)
                return $"Hace {(int)diff.TotalMinutes} minuto{((int)diff.TotalMinutes != 1 ? "s" : "")}";

            if (diff.TotalHours < 24)
                return $"Hace {(int)diff.TotalHours} hora{((int)diff.TotalHours != 1 ? "s" : "")}";

            if (diff.TotalDays < 7)
                return $"Hace {(int)diff.TotalDays} día{((int)diff.TotalDays != 1 ? "s" : "")}";

            return FormatDate(dateTime);
        }

        public static string GetDayOfWeek(DateTime date)
        {
            return date.ToString("dddd", EcuadorCulture);
        }

        public static string GetMonthName(DateTime date)
        {
            return date.ToString("MMMM", EcuadorCulture);
        }

        public static bool IsToday(DateTime date)
        {
            return date.Date == DateTime.Today;
        }

        public static bool IsTomorrow(DateTime date)
        {
            return date.Date == DateTime.Today.AddDays(1);
        }

        public static bool IsThisWeek(DateTime date)
        {
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);
            return date.Date >= startOfWeek && date.Date < endOfWeek;
        }

        public static string GetFriendlyDate(DateTime date)
        {
            if (IsToday(date))
                return "Hoy";

            if (IsTomorrow(date))
                return "Mañana";

            if (IsThisWeek(date))
                return GetDayOfWeek(date);

            return FormatDate(date);
        }

        public static TimeSpan GetTimeUntil(DateTime futureDate)
        {
            return futureDate - DateTime.Now;
        }

        public static string GetTimeUntilString(DateTime futureDate)
        {
            var timeSpan = GetTimeUntil(futureDate);

            if (timeSpan.TotalDays >= 1)
                return $"En {(int)timeSpan.TotalDays} día{((int)timeSpan.TotalDays != 1 ? "s" : "")}";

            if (timeSpan.TotalHours >= 1)
                return $"En {(int)timeSpan.TotalHours} hora{((int)timeSpan.TotalHours != 1 ? "s" : "")}";

            if (timeSpan.TotalMinutes >= 1)
                return $"En {(int)timeSpan.TotalMinutes} minuto{((int)timeSpan.TotalMinutes != 1 ? "s" : "")}";

            return "Muy pronto";
        }
    }
}