using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore
{
  public static class SecureHasherV1
  {
    /// <summary>
    /// Size of salt.
    /// </summary>
    //private const int SaltSize = 16;

    /// <summary>
    /// Size of hash.
    /// </summary>
    //private const int HashSize = 20;

    private const string DELIMITTER = "~";
    private const string HASH_HEADER_V1 = "BAJL_HASH" + DELIMITTER + "V1" + DELIMITTER;

    /// <summary>
    /// Creates a hash from a password.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="iterations">Number of iterations.</param>
    /// <returns>The hash.</returns>
    public static string Hash(string password)
    {
      int saltSize = Options.GetSettings.SettingGetAsInt("SaltSize");
      int hashSize = Options.GetSettings.SettingGetAsInt("HashSize");
      int hashIterations = Options.GetSettings.SettingGetAsInt("HashInterations");
      // Create salt
      byte[] salt = RandomNumberGenerator.GetBytes(saltSize);

      // Create hash
      var pbkdf2 = new Rfc2898DeriveBytes(password, salt, hashIterations);
      var hash = pbkdf2.GetBytes(hashSize);

      // Combine salt and hash
      var hashBytes = new byte[saltSize + hashSize];
      Array.Copy(salt, 0, hashBytes, 0, saltSize);
      Array.Copy(hash, 0, hashBytes, saltSize, hashSize);

      // Convert to base64
      var base64Hash = Convert.ToBase64String(hashBytes);

      // Format hash with extra information
      return $"{HASH_HEADER_V1}{base64Hash}";
    }


    /// <summary>
    /// Checks if hash is supported.
    /// </summary>
    /// <param name="hashString">The hash.</param>
    /// <returns>Is supported?</returns>
    public static bool IsHashSupported(string hashString)
    {
      return hashString.StartsWith(HASH_HEADER_V1);
    }

    public static string SessionIdCreate()
    {
      byte[] randomBytes = RandomNumberGenerator.GetBytes(Options.GetSettings.SettingGetAsInt("SessionSize"));
      return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// Verifies a password against a hash.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="hashedPassword">The hash.</param>
    /// <returns>Could be verified?</returns>
    public static bool Verify(string password, string hashedPassword)
    {
      int saltSize = Options.GetSettings.SettingGetAsInt("SaltSize");
      int hashSize = Options.GetSettings.SettingGetAsInt("HashSize");
      int hashIterations = Options.GetSettings.SettingGetAsInt("HashInterations");

      // Check hash
      if (!IsHashSupported(hashedPassword))
      {
        throw new NotSupportedException("The hashtype is not supported");
      }

      // Extract iteration and Base64 string
      var splittedHashString = hashedPassword.Split(DELIMITTER);
      //var iterations = int.Parse(splittedHashString[0]);
      var base64Hash = splittedHashString[2];

      // Get hash bytes
      var hashBytes = Convert.FromBase64String(base64Hash);

      // Get salt
      var salt = new byte[saltSize];
      Array.Copy(hashBytes, 0, salt, 0, saltSize);

      // Create hash with given salt
      var pbkdf2 = new Rfc2898DeriveBytes(password, salt, hashIterations);
      byte[] hash = pbkdf2.GetBytes(hashSize);

      // Get result
      for (var i = 0; i < hashSize; i++)
      {
        if (hashBytes[i + saltSize] != hash[i])
        {
          return false;
        }
      }
      return true;
    }
  }
}
