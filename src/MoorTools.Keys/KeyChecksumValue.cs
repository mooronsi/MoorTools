using System.Security.Cryptography;

namespace MoorTools.Keys;

/// <summary>
/// Provides methods for calculating the Key Check Value (KCV) for cryptographic keys using TripleDES encryption.
/// The KCV is a value used to verify the correctness of a cryptographic key without exposing the key itself.
/// </summary>
public static class KeyChecksumValue
{
    /// <summary>
    /// Calculates the Key Check Value (KCV) for the provided key using TripleDES encryption.
    /// The KCV is derived by encrypting a block of zeros and returning the first three bytes of the ciphertext.
    /// </summary>
    /// <param name="key">The cryptographic key, which must be either 16 or 24 bytes long (128 or 192 bits).</param>
    /// <returns>A byte array containing the first three bytes of the encrypted zero block, representing the KCV.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the provided key is not 16 or 24 bytes long.
    /// </exception>
    public static byte[] Calc(byte[] key)
    {
        if (key.Length != 16 && key.Length != 24)
            throw new ArgumentException("Key must be 128 or 192 bits long.", nameof(key));

        using var tDes = TripleDES.Create();
        tDes.Key = key;
        tDes.Mode = CipherMode.ECB;
        tDes.Padding = PaddingMode.None;

        return tDes.CreateEncryptor().TransformFinalBlock(new byte[8], 0, 8)[..3];
    }
}