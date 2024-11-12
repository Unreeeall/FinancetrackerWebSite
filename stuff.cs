using System;
using System.Security.Cryptography;

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
}