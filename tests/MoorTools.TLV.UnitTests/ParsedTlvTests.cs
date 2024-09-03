using MoorTools.TLV.Models;

namespace MoorTools.TLV.UnitTests;

[TestFixture]
public class ParsedTlvTests
{
    [Test]
    public void Constructor_ShouldInitializeValue_WhenGivenValidList()
    {
        // Arrange
        var tlvList = new List<Tlv>
        {
            new() { Tag = 1, Data = [0x01] },
            new() { Tag = 2, Data = [0x02] }
        };

        // Act
        var parsedTlv = new ParsedTlv(tlvList);

        // Assert
        Assert.That(parsedTlv.Value, Is.EqualTo(tlvList));
    }

    [Test]
    public void Constructor_ShouldThrowArgumentNullException_WhenValueIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => { _ = new ParsedTlv(null!); });
    }

    [Test]
    public void ToString_ShouldReturnFormattedString_WhenTlvIsPrimitive()
    {
        // Arrange
        var tlvList = new List<Tlv>
        {
            new() { Tag = 1, Data = [0x01, 0x02] }
        };
        var parsedTlv = new ParsedTlv(tlvList);

        // Act
        var result = parsedTlv.ToString();

        // Assert
        Assert.That(result, Is.EqualTo("Tag: 01 - Data: 0102\r\n"));
    }

    [Test]
    public void ToString_ShouldReturnFormattedString_WhenTlvIsConstructed()
    {
        // Arrange
        var tlvList = new List<Tlv>
        {
            new()
            {
                Tag = 1,
                IsConstructed = true,
                Children = new List<Tlv>
                {
                    new() { Tag = 2, Data = [0x02] }
                }
            }
        };
        var parsedTlv = new ParsedTlv(tlvList);

        // Act
        var result = parsedTlv.ToString();

        // Assert
        Assert.That(result, Is.EqualTo("Tag: 01\r\n--Tag: 02 - Data: 02\r\n"));
    }

    [Test]
    public void ToString_ShouldThrowException_WhenConstructedTlvHasNoChildren()
    {
        // Arrange
        var tlvList = new List<Tlv>
        {
            new()
            {
                Tag = 1,
                IsConstructed = true,
                Children = new List<Tlv>()
            }
        };
        var parsedTlv = new ParsedTlv(tlvList);

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => { _ = parsedTlv.ToString(); });
        Assert.That(ex.Message, Is.EqualTo("No children found for constructed TLV."));
    }

    [Test]
    public void ToString_ShouldThrowException_WhenPrimitiveTlvHasNoData()
    {
        // Arrange
        var tlvList = new List<Tlv>
        {
            new() { Tag = 1 }
        };
        var parsedTlv = new ParsedTlv(tlvList);

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => { _ = parsedTlv.ToString(); });
        Assert.That(ex.Message, Is.EqualTo("No data available for this TLV."));
    }
}