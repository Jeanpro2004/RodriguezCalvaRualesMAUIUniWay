using System.Text.RegularExpressions;

namespace RodriguezCalvaRualesMAUIUniWay.Utils
{
    public static class ValidationHelper
    {
        // Email validation
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailRegex);
        }

        public static bool IsUniversityEmail(string email)
        {
            if (!IsValidEmail(email))
                return false;

            var universityDomains = new[] { ".edu.ec", ".edu", ".ac.ec" };
            return universityDomains.Any(domain => email.EndsWith(domain, StringComparison.OrdinalIgnoreCase));
        }

        // Phone validation
        public static bool IsValidEcuadorianPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Remove spaces and format characters
            var cleanPhone = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

            // Check if starts with +593 or 593
            if (cleanPhone.StartsWith("+593"))
                cleanPhone = cleanPhone.Substring(4);
            else if (cleanPhone.StartsWith("593"))
                cleanPhone = cleanPhone.Substring(3);

            // Should be 9 digits for mobile (09xxxxxxxx) or 8 for landline
            return cleanPhone.Length == 9 && cleanPhone.StartsWith("9") ||
                   cleanPhone.Length == 8 && !cleanPhone.StartsWith("9");
        }

        // ID Banner validation
        public static bool IsValidIdBanner(string idBanner)
        {
            if (string.IsNullOrWhiteSpace(idBanner))
                return false;

            // Format: B00xxxxxx (B followed by 8 digits)
            var bannerRegex = @"^[Bb]\d{8}$";
            return Regex.IsMatch(idBanner, bannerRegex);
        }

        // Password validation
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return password.Length >= 8;
        }

        public static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(c => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c));

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        // Name validation
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return name.Length >= 2 && name.Length <= 50 &&
                   name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }

        // License plate validation (Ecuador)
        public static bool IsValidEcuadorianPlate(string plate)
        {
            if (string.IsNullOrWhiteSpace(plate))
                return false;

            // Format: ABC-1234 or AB-1234
            var plateRegex = @"^[A-Z]{2,3}-\d{3,4}$";
            return Regex.IsMatch(plate.ToUpper(), plateRegex);
        }

        // Price validation
        public static bool IsValidPrice(decimal price)
        {
            return price > 0 && price <= 100; // Reasonable price range for rides
        }

        // Date validation
        public static bool IsValidTripDate(DateTime date)
        {
            return date.Date >= DateTime.Today && date.Date <= DateTime.Today.AddDays(30);
        }

        // Time validation
        public static bool IsValidTripTime(TimeSpan time)
        {
            return time >= TimeSpan.FromHours(5) && time <= TimeSpan.FromHours(23);
        }

        // Capacity validation
        public static bool IsValidCapacity(int capacity)
        {
            return capacity >= 1 && capacity <= 8; // Reasonable range for vehicle capacity
        }
    }
}

