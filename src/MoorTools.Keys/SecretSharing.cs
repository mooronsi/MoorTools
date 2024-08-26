using System.Security.Cryptography;

namespace MoorTools.Keys;

/// <summary>
/// Provides methods for secure secret sharing using various techniques.
/// </summary>
public static class SecretSharing
{
    /// <summary>
    /// Provides methods for secret sharing using the XOR operation. 
    /// This class contains functionality to split a secret into multiple parts 
    /// and to combine those parts to reconstruct the original secret.
    /// </summary>
    public static class Xor
    {
        /// <summary>
        /// Combines the specified parts to reconstruct the original key using XOR operation.
        /// </summary>
        /// <param name="partCount">The number of parts that make up the key. This should match the number of arrays provided in <paramref name="parts"/>.</param>
        /// <param name="parts">An array of byte arrays representing the parts of the key. All parts must be of the same length.</param>
        /// <returns>A byte array representing the reconstructed key.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><description><paramref name="parts"/> is empty.</description></item>
        /// <item><description>Any of the parts in <paramref name="parts"/> have differing lengths.</description></item>
        /// </list>
        /// </exception>
        public static byte[] CombineKey(int partCount, params byte[][] parts)
        {
            if (parts.Length == 0)
                throw new ArgumentException("At least one part is required.", nameof(parts));

            var keyLength = parts[0].Length;

            if (parts.Any(part => part.Length != keyLength))
                throw new ArgumentException("All parts must have the same length.", nameof(parts));

            var key = new byte[keyLength];
            for (var i = 0; i < key.Length; i++)
            {
                byte result = 0;
                for (var j = 0; j < partCount; j++)
                    result ^= parts[j][i];
                key[i] = result;
            }

            return key;
        }

        /// <summary>
        /// Splits the given key into the specified number of parts using XOR operation, 
        /// ensuring that the original key can be reconstructed by combining all parts.
        /// </summary>
        /// <param name="partCount">The total number of parts to split the key into. Must be at least 2.</param>
        /// <param name="key">The byte array representing the key to be split. The key must not be empty.</param>
        /// <returns>
        /// An array of byte arrays, where each element represents a part of the original key.
        /// The original key can be reconstructed by combining all returned parts using the XOR operation.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><description><paramref name="partCount"/> is less than 2.</description></item>
        /// <item><description><paramref name="key"/> is empty.</description></item>
        /// </list>
        /// </exception>
        public static byte[][] SplitKey(int partCount, byte[] key)
        {
            if (partCount < 2)
                throw new ArgumentException("At least two parts are required.", nameof(partCount));

            if (key.Length == 0)
                throw new ArgumentException("Key must not be empty.", nameof(key));

            var parts = new byte[partCount][];

            for (var i = 0; i < partCount - 1; i++)
                parts[i] = RandomNumberGenerator.GetBytes(key.Length);

            parts[partCount - 1] = new byte[key.Length];
            for (var i = 0; i < key.Length; i++)
            {
                byte result = 0;
                for (var j = 0; j < partCount - 1; j++)
                    result ^= parts[j][i];
                parts[partCount - 1][i] = (byte)(result ^ key[i]);
            }

            return parts;
        }
    }
}