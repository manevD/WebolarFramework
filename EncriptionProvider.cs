using System;
using System.Security.Cryptography;
using System.Text;

namespace Webolar.Framework;

public class EncriptionProvider
{
    public enum Supported_HA
    {
        SHA256,
        SHA384,
        SHA512
    }

    /// <summary>
    ///     Encripts a string with salted SHA hashing.
    /// </summary>
    /// <param name="plainText">The original string</param>
    /// <param name="hash">The type of the hashing: SHA-256 / SHA-384 / SHA-512 </param>
    /// <param name="salt">
    ///     If this parameter is set to null, the function generates a random salt array.
    ///     If every password is encripted with a random salt array, it is harder to read
    ///     out from the database which users use the same passwords.
    /// </param>
    /// <returns></returns>
    public static string ComputeHash(string plainText, Supported_HA hash, byte[] salt)
    {
        if (plainText == null) plainText = "";

        int minSaltLength = 4, maxSaltLength = 16;

        byte[] SaltBytes = null;
        if (salt != null)
        {
            SaltBytes = salt;
        }
        else
        {
            var r = new Random();
            var SaltLength = r.Next(minSaltLength, maxSaltLength);
            SaltBytes = new byte[SaltLength];
            var rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(SaltBytes);
            rng.Dispose();
        }

        var plainData = Encoding.UTF8.GetBytes(plainText);
        var plainDataWithSalt = new byte[plainData.Length + SaltBytes.Length];

        for (var x = 0; x < plainData.Length; x++)
            plainDataWithSalt[x] = plainData[x];
        for (var n = 0; n < SaltBytes.Length; n++)
            plainDataWithSalt[plainData.Length + n] = SaltBytes[n];

        byte[] hashValue = null;

        switch (hash)
        {
            case Supported_HA.SHA256:
                var sha = new SHA256Managed();
                hashValue = sha.ComputeHash(plainDataWithSalt);
                sha.Dispose();
                break;
            case Supported_HA.SHA384:
                var sha1 = new SHA384Managed();
                hashValue = sha1.ComputeHash(plainDataWithSalt);
                sha1.Dispose();
                break;
            case Supported_HA.SHA512:
                var sha2 = new SHA512Managed();
                hashValue = sha2.ComputeHash(plainDataWithSalt);
                sha2.Dispose();
                break;
        }

        var result = new byte[hashValue.Length + SaltBytes.Length];
        for (var x = 0; x < hashValue.Length; x++)
            result[x] = hashValue[x];
        for (var n = 0; n < SaltBytes.Length; n++)
            result[hashValue.Length + n] = SaltBytes[n];

        return Convert.ToBase64String(result);
    }

    /// <summary>
    ///     Checks if a plainText is the same as the original text which was encripted to the hashValue.
    /// </summary>
    /// <param name="plainText">Original text.</param>
    /// <param name="hashValue">Encripted text.</param>
    /// <param name="hash">
    ///     The type of the hashing: SHA-256 / SHA-384 / SHA-512
    ///     You have to select the same method that created the hashValue.
    /// </param>
    /// <returns></returns>
    public static bool Confirm(string plainText, string hashValue, Supported_HA hash)
    {
        if (plainText == null) plainText = "";
        var hashBytes = Convert.FromBase64String(hashValue);
        var hashSize = 0;

        switch (hash)
        {
            case Supported_HA.SHA256:
                hashSize = 32;
                break;
            case Supported_HA.SHA384:
                hashSize = 48;
                break;
            case Supported_HA.SHA512:
                hashSize = 64;
                break;
        }

        var saltBytes = new byte[hashBytes.Length - hashSize];

        for (var x = 0; x < saltBytes.Length; x++)
            saltBytes[x] = hashBytes[hashSize + x];

        var newHash = ComputeHash(plainText, hash, saltBytes);

        return hashValue == newHash;
    }
}