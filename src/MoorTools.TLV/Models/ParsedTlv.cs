using System.Text;

namespace MoorTools.TLV.Models;

/// <summary>
/// Initializes a new instance of the <see cref="ParsedTlv"/> class with a specified list of TLV (Tag-Length-Value) objects.
/// </summary>
/// <param name="value">
/// The list of <see cref="Tlv"/> objects representing the parsed TLV structure. 
/// This value cannot be null.
/// </param>
/// <exception cref="ArgumentNullException">
/// Thrown if the <paramref name="value"/> parameter is null.
/// </exception>
public class ParsedTlv(List<Tlv> value)
{
    /// <summary>
    /// Gets the list of TLV (Tag-Length-Value) items associated with this ParsedTlv instance.
    /// This list is initialized during object construction and cannot be null.
    /// </summary>
    public List<Tlv> Value { get; } = value ?? throw new ArgumentNullException(nameof(value));

    /// <summary>
    /// Converts the TLV (Tag-Length-Value) structure into a human-readable string format.
    /// This string representation includes the tag, data, and any nested child TLVs.
    /// </summary>
    /// <returns>
    /// A string representing the TLV structure, with each tag and its corresponding data 
    /// or nested TLVs formatted in a readable manner.
    /// </returns>
    /// <exception cref="Exception">
    /// Thrown when a constructed TLV does not have any children or when a primitive TLV does not have data.
    /// </exception>
    public override string ToString()
    {
        var sb = new StringBuilder();

        foreach (var item in Value)
            Prepare(item);

        return sb.ToString();

        // Recursively prepares a string representation of a TLV and its children
        void Prepare(Tlv tlv, string prefix = "")
        {
            sb.Append(prefix == "" ? $"Tag: {tlv.Tag:X2}" : $"{prefix}Tag: {tlv.Tag:X2}");

            if (tlv.IsConstructed)
            {
                sb.AppendLine();

                if (tlv.Children is null || tlv.Children.Count == 0)
                    throw new Exception("No children found for constructed TLV.");

                prefix += "--";

                foreach (var child in tlv.Children)
                    Prepare(child, prefix);
            }
            else
            {
                sb.AppendLine($" - Data: {Convert.ToHexString(tlv.Data
                                                              ?? throw new Exception("No data available for this TLV."))}");
            }
        }
    }
}