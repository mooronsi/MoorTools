using MoorTools.TLV.Models;

namespace MoorTools.TLV.Extensions;

/// <summary>
/// Provides extension methods for working with ParsedTlv objects.
/// </summary>
public static class ParsedTlvExtensions
{
    /// <summary>
    /// Finds all TLV objects with the specified tag within the ParsedTlv.
    /// </summary>
    /// <param name="parsedTlv">The ParsedTlv instance to search within.</param>
    /// <param name="tag">The tag value to search for.</param>
    /// <returns>A list of TLV objects that match the specified tag.</returns>
    public static List<Tlv> FindTag(this ParsedTlv parsedTlv, int tag)
    {
        return FindTagInternal(tag, parsedTlv.Value);
    }

    /// <summary>
    /// Recursively searches for TLV objects with the specified tag within a list of TLV objects.
    /// </summary>
    /// <param name="tag">The tag value to search for.</param>
    /// <param name="tlvList">The list of TLV objects to search within.</param>
    /// <returns>A list of TLV objects that match the specified tag.</returns>
    private static List<Tlv> FindTagInternal(int tag, List<Tlv> tlvList)
    {
        var result = new List<Tlv>();

        foreach (var tlv in tlvList)
        {
            if (tlv.Tag == tag) result.Add(tlv);
            if (tlv is not { IsConstructed: true, Children: not null }) continue;
            var childResults = FindTagInternal(tag, tlv.Children);
            result.AddRange(childResults);
        }

        return result;
    }
}