using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core
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
    private const string HASH_HEADER = "BAJL_HASH" + DELIMITTER + "V1" + DELIMITTER;

    /// <summary>
    /// Creates a hash from a password.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="iterations">Number of iterations.</param>
    /// <returns>The hash.</returns>
    public static string Hash(string password)
    {
      // Create salt
      byte[] salt = RandomNumberGenerator.GetBytes(Options.AdministrationSaltSize);

      // Create hash
      var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Options.AdministrationHashInterations);
      var hash = pbkdf2.GetBytes(Options.AdministrationHashSize);

      // Combine salt and hash
      var hashBytes = new byte[Options.AdministrationSaltSize + Options.AdministrationHashSize];
      Array.Copy(salt, 0, hashBytes, 0, Options.AdministrationSaltSize);
      Array.Copy(hash, 0, hashBytes, Options.AdministrationSaltSize, Options.AdministrationHashSize);

      // Convert to base64
      var base64Hash = Convert.ToBase64String(hashBytes);

      // Format hash with extra information
      return $"{HASH_HEADER}{Options.AdministrationHashInterations}{DELIMITTER}{base64Hash}";
    }


    /// <summary>
    /// Checks if hash is supported.
    /// </summary>
    /// <param name="hashString">The hash.</param>
    /// <returns>Is supported?</returns>
    public static bool IsHashSupported(string hashString)
    {
      return hashString.Contains(HASH_HEADER);
    }

    /// <summary>
    /// Verifies a password against a hash.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="hashedPassword">The hash.</param>
    /// <returns>Could be verified?</returns>
    public static bool Verify(string password, string hashedPassword)
    {
      // Check hash
      if (!IsHashSupported(hashedPassword))
      {
        throw new NotSupportedException("The hashtype is not supported");
      }

      // Extract iteration and Base64 string
      var splittedHashString = hashedPassword.Replace(HASH_HEADER, "").Split(DELIMITTER);
      var iterations = int.Parse(splittedHashString[0]);
      var base64Hash = splittedHashString[1];

      // Get hash bytes
      var hashBytes = Convert.FromBase64String(base64Hash);

      // Get salt
      var salt = new byte[Options.AdministrationSaltSize];
      Array.Copy(hashBytes, 0, salt, 0, Options.AdministrationSaltSize);

      // Create hash with given salt
      var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
      byte[] hash = pbkdf2.GetBytes(Options.AdministrationHashSize);

      // Get result
      for (var i = 0; i < Options.AdministrationHashSize; i++)
      {
        if (hashBytes[i + Options.AdministrationSaltSize] != hash[i])
        {
          return false;
        }
      }
      return true;
    }
  }
}
