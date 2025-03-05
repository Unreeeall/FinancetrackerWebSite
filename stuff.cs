using System.Security.Cryptography;
using System.Globalization;

namespace SuperUsefullOrSo
{
    public class Stuff
    {
        public static string GenerateRandomBase64String(int length)
        {
            byte[] randomBytes = new byte[length];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }
        public static string MonthToWord(int month)
        {
            CultureInfo englishCulture = new CultureInfo("en-US");
            string monthName = englishCulture.DateTimeFormat.GetMonthName(month);
            return monthName;
        }
    }
}
