using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FlowEngineCore
{
  public static class SecureHasher
  {
    public enum VERSION
    {
      _Unsupported,
      V1,
      V2
    }


    private const string DELIMITTER = "~";
    private const string HASH_HEADER_V1 = "BAJL_HASH" + DELIMITTER + "V1" + DELIMITTER;
    private const string HASH_HEADER_V2 = "BAJL_HASH" + DELIMITTER + "V2" + DELIMITTER;

    /// <summary>
    /// Creates a hash from a password.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <returns>The hash.</returns>
    public static string Hash(string password)
    {
      int saltSize = Options.GetSettings.SettingGetAsInt("SaltSize");
      int hashSize = Options.GetSettings.SettingGetAsInt("HashSize");
      int hashIterations = Options.GetSettings.SettingGetAsInt("HashInterations");
      // Create salt
      byte[] salt = RandomNumberGenerator.GetBytes(saltSize);

      // Create hash
      var pbkdf2 = new Rfc2898DeriveBytes(password, salt, hashIterations, HashAlgorithmName.SHA512);
      var hash = pbkdf2.GetBytes(hashSize);

      // Combine salt and hash
      var hashBytes = new byte[saltSize + hashSize];
      Array.Copy(salt, 0, hashBytes, 0, saltSize);
      Array.Copy(hash, 0, hashBytes, saltSize, hashSize);

      // Convert to base64
      var base64Hash = Convert.ToBase64String(hashBytes);

      // Format hash with extra information
      return $"{HASH_HEADER_V2}{base64Hash}";
    }


    /// <summary>
    /// Checks if hash is supported.
    /// </summary>
    /// <param name="hashString">The hash.</param>
    /// <returns>Is supported?</returns>
    public static VERSION GetHashVersion(string hashString)
    {
      if (hashString.StartsWith(HASH_HEADER_V2) == true)
        return VERSION.V2;
      else if (hashString.StartsWith(HASH_HEADER_V1) == true)
        return VERSION.V1;
      else
        return VERSION._Unsupported;
    }

    public static string SessionIdCreate()
    {
      byte[] randomBytes = RandomNumberGenerator.GetBytes(Options.GetSettings.SettingGetAsInt("SessionSize"));
      return Convert.ToBase64String(randomBytes);
    }

    public static byte[] RandomByte(int length)
    {
      return RandomNumberGenerator.GetBytes(length);
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

      VERSION hashVersion = GetHashVersion(hashedPassword);
      // Check hash
      if (hashVersion == VERSION._Unsupported)
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
      Rfc2898DeriveBytes pbkdf2;
      if (hashVersion == VERSION.V1)
        pbkdf2 = new Rfc2898DeriveBytes(password, salt, hashIterations);
      else
        pbkdf2 = new Rfc2898DeriveBytes(password, salt, hashIterations, HashAlgorithmName.SHA512);

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
