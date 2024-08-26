using System.Security.Cryptography;

namespace MoorTools.Keys;

/// <summary>
/// Provides methods for generating cryptographic keys of different lengths for use with encryption algorithms,
/// particularly TripleDES. The keys can be generated with single, double, or triple lengths.
/// </summary>
public static class KeyGenerator
{
    /// <summary>
    /// Specifies the length of cryptographic keys for use with encryption algorithms, 
    /// particularly TripleDES. The key length can be single, double, or triple, 
    /// corresponding to 8, 16, or 24 bytes respectively.
    /// </summary>
    public enum KeyLength
    {
        /// <summary>
        /// Single-length key, 8 bytes (64 bits), used for basic encryption.
        /// </summary>
        Single = 8,

        /// <summary>
        /// Double-length key, 16 bytes (128 bits), used for enhanced security with double encryption.
        /// </summary>
        Double = 16,

        /// <summary>
        /// Triple-length key, 24 bytes (192 bits), used for maximum security with triple encryption.
        /// </summary>
        Triple = 24,
    }

    /// <summary>
    /// Generates a cryptographic key of the specified length using a secure random number generator.
    /// The generated key can be of single, double, or triple length suitable for use with TripleDES encryption.
    /// </summary>
    /// <param name="length">The desired length of the key, typically 8, 16, or 24 bytes, corresponding to single, double, or triple length keys.</param>
    /// <returns>A byte array containing the securely generated key of the specified length.</returns>
    public static byte[] GenerateKey(KeyLength length)
    {
        if (!Enum.IsDefined(typeof(KeyLength), length))
            throw new ArgumentException("Invalid key length specified.", nameof(length));

        var key = new byte[(int)length];
        key = RandomNumberGenerator.GetBytes(key.Length);
        return key;
    }
}