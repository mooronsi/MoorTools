namespace MoorTools.TLV.Models;

/// <summary>
/// Represents a TLV (Tag-Length-Value) structure commonly used in binary protocols
/// to encode optional information elements.
/// </summary>
public class Tlv
{
    /// <summary>
    /// Gets or sets the tag that indicates the type of data stored in this TLV.
    /// </summary>
    public int Tag { get; set; }

    /// <summary>
    /// Gets or sets the length of the data. For primitive TLVs, this is the length of the 
    /// <see cref="Data"/> array. For constructed TLVs, this may represent the sum of the lengths 
    /// of all child TLVs.
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    /// Gets or sets the value associated with this TLV. This is null if the TLV is constructed.
    /// </summary>
    public byte[]? Data { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this TLV is constructed (i.e., it contains 
    /// other TLVs as children) or primitive (i.e., it contains raw data).
    /// </summary>
    public bool IsConstructed { get; set; }

    /// <summary>
    /// Gets or sets the list of child TLVs. This is only relevant if <see cref="IsConstructed"/> 
    /// is true. If the TLV is primitive, this value is null.
    /// </summary>
    public List<Tlv>? Children { get; set; }
}