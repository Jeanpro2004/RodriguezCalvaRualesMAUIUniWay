using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RodriguezCalvaRualesMAUIUniWay.Models
{
    public class LoginAttempt
    {
        public string Email { get; set; }
        public DateTime AttemptDateTime { get; set; }
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public string IpAddress { get; set; }
        public string DevicePlatform { get; set; }
        public int AttemptNumber { get; set; }

        public LoginAttempt()
        {
            AttemptDateTime = DateTime.Now;
            IpAddress = "Local";
            DevicePlatform = GetDevicePlatform();
        }

        public LoginAttempt(string email, bool isSuccessful, string errorMessage = "", int attemptNumber = 1)
        {
            Email = email;
            IsSuccessful = isSuccessful;
            ErrorMessage = errorMessage ?? "";
            AttemptNumber = attemptNumber;
            AttemptDateTime = DateTime.Now;
            IpAddress = "Local";
            DevicePlatform = GetDevicePlatform();
        }

        private static string GetDevicePlatform()
        {
            try
            {
                return DeviceInfo.Current?.Platform.ToString() ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        public override string ToString()
        {
            return $"{AttemptDateTime:yyyy-MM-dd HH:mm:ss}|{Email}|{IsSuccessful}|{ErrorMessage}|{IpAddress}|{DevicePlatform}|{AttemptNumber}";
        }

        public static LoginAttempt FromString(string line)
        {
            try
            {
                var parts = line.Split('|');
                if (parts.Length >= 7)
                {
                    return new LoginAttempt
                    {
                        AttemptDateTime = DateTime.Parse(parts[0]),
                        Email = parts[1],
                        IsSuccessful = bool.Parse(parts[2]),
                        ErrorMessage = parts[3],
                        IpAddress = parts[4],
                        DevicePlatform = parts[5],
                        AttemptNumber = int.Parse(parts[6])
                    };
                }
            }
            catch (Exception)
            {
                
            }
            return null;
        }

        public string GetStatusText()
        {
            return IsSuccessful ? "Exitoso" : "Fallido";
        }

        public string GetFormattedDateTime()
        {
            return AttemptDateTime.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}