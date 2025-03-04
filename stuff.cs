using System.Security.Cryptography;
using System.Globalization;

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

    // CultureInfo englishCulture = new CultureInfo("en-US");
    public static string MonthToWord(int month)
    {
        CultureInfo englishCulture = new CultureInfo("en-US");
        string monthName = englishCulture.DateTimeFormat.GetMonthName(month);
        return monthName;
    }
}