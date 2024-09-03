using System.Buffers;
using MoorTools.TLV.Models;

namespace MoorTools.TLV;

/// <summary>
/// Provides methods for parsing TLV (Tag-Length-Value) encoded data from hexadecimal strings and byte arrays.
/// </summary>
public static class TlvParser
{
    /// <summary>
    /// Parses a TLV structure from a hexadecimal string.
    /// </summary>
    /// <param name="hex">The hexadecimal string representing the TLV data.</param>
    /// <returns>A <see cref="ParsedTlv"/> object representing the parsed TLV structure.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="hex"/> is null, empty, or whitespace, or if it does not have an even number of characters.
    /// </exception>
    public static ParsedTlv Parse(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(hex));

        if (hex.Length % 2 != 0)
            throw new ArgumentException("Value must have an even number of characters.", nameof(hex));

        return new ParsedTlv(ParseTlv(Convert.FromHexString(hex)));
    }

    /// <summary>
    /// Parses a TLV structure from a byte array.
    /// </summary>
    /// <param name="tlv">The byte array representing the TLV data.</param>
    /// <returns>A <see cref="ParsedTlv"/> object representing the parsed TLV structure.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="tlv"/> is null, empty, or has an odd number of bytes.
    /// </exception>
    public static ParsedTlv Parse(byte[] tlv)
    {
        if (tlv is null || tlv.Length == 0)
            throw new ArgumentException("Value must not be null or empty.", nameof(tlv));

        if (tlv.Length % 2 != 0)
            throw new ArgumentException("Value must have an even number of bytes.", nameof(tlv));

        return new ParsedTlv(ParseTlv(tlv));
    }

    /// <summary>
    /// Reads the tag from the TLV data stream.
    /// </summary>
    /// <param name="reader">The sequence reader used to read the TLV data.</param>
    /// <param name="isConstructed">Outputs whether the TLV is constructed.</param>
    /// <returns>An integer representing the tag.</returns>
    private static int GetTag(ref SequenceReader<byte> reader, out bool isConstructed)
    {
        int tag = reader.UnreadSpan[0];

        // Determine if the TLV is constructed by checking the 6th bit of the tag (bitmask 0x20).
        isConstructed = (tag & 0x20) == 0x20;
        reader.Advance(1);

        // Check if the tag is a multi-byte tag (indicated by the value 0x1F in the lower 5 bits).
        if ((tag & 0x1F) != 0x1F) return tag; // If it's not a multi-byte tag, return the tag.

        while (true)
        {
            var next = reader.UnreadSpan[0];

            // Shift the current tag to the left by 8 bits and add the next byte.
            tag = (tag << 8) | next;
            reader.Advance(1);

            // If the high-order bit (0x80) is not set, this is the last byte of the tag, so break the loop.
            if ((next & 0x80) == 0) break;
        }

        return tag;
    }

    /// <summary>
    /// Reads the length from the TLV data stream.
    /// </summary>
    /// <param name="reader">The sequence reader used to read the TLV data.</param>
    /// <returns>An integer representing the length.</returns>
    private static int GetLength(ref SequenceReader<byte> reader)
    {
        int length = reader.UnreadSpan[0];
        reader.Advance(1);

        switch (length)
        {
            // If the length byte is 0x81, the length is encoded in the next byte (1 additional byte).
            case 0x81:
                length = reader.UnreadSpan[0];
                reader.Advance(1);
                break;
            // If the length byte is 0x82, the length is encoded in the next two bytes (2 additional bytes).
            case 0x82:
                length = (reader.UnreadSpan[0] << 8) | reader.UnreadSpan[1];
                reader.Advance(2);
                break;
            // If the length byte is 0x83, the length is encoded in the next three bytes (3 additional bytes).
            case 0x83:
                length = (reader.UnreadSpan[0] << 16) | (reader.UnreadSpan[1] << 8) | reader.UnreadSpan[2];
                reader.Advance(3);
                break;
            // If the length byte is 0x84, the length is encoded in the next four bytes (4 additional bytes).
            case 0x84:
                length = (reader.UnreadSpan[0] << 24) | (reader.UnreadSpan[1] << 16) |
                         (reader.UnreadSpan[2] << 8) |
                         reader.UnreadSpan[3];
                reader.Advance(4);
                break;
        }

        return length;
    }

    /// <summary>
    /// Reads the data from the TLV data stream.
    /// </summary>
    /// <param name="reader">The sequence reader used to read the TLV data.</param>
    /// <param name="length">The length of the data to read.</param>
    /// <returns>A span of bytes representing the data.</returns>
    private static ReadOnlySpan<byte> GetData(ref SequenceReader<byte> reader, int length)
    {
        var data = reader.UnreadSpan[..length];
        reader.Advance(length);
        return data;
    }

    /// <summary>
    /// Recursively parses a TLV structure from a byte array.
    /// </summary>
    /// <param name="tlv">The byte array representing the TLV data.</param>
    /// <returns>A list of <see cref="Tlv"/> objects representing the parsed TLV structure.</returns>
    /// <exception cref="Exception">Thrown if a constructed TLV has no children or if an error occurs while parsing children.</exception>
    private static List<Tlv> ParseTlv(byte[] tlv)
    {
        var result = new List<Tlv>();

        var sequence = new ReadOnlySequence<byte>(tlv);
        var reader = new SequenceReader<byte>(sequence);

        while (true)
        {
            if (reader.Remaining == 0) break;

            var current = new Tlv
            {
                Tag = GetTag(ref reader, out var isConstructed),
                IsConstructed = isConstructed,
                Length = GetLength(ref reader)
            };
            current.Data = GetData(ref reader, current.Length).ToArray();

            if (isConstructed)
            {
                if (current.Data is null)
                    throw new Exception("Constructed TLV has no data.");

                current.Children = ParseTlv(current.Data) ?? throw new Exception("TLV children parse error");
            }

            result.Add(current);
        }

        return result;
    }
}