using MoorTools.TLV.Models;
using MoorTools.TLV.UnitTests.Data;

namespace MoorTools.TLV.UnitTests;

[TestFixture]
public class TlvParserTests
{
    [Test]
    public void Parse_FromHexString_ThrowsArgumentException_WhenHexIsNullOrEmpty()
    {
        // Arrange
        const string? hex = null;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => TlvParser.Parse(hex!));
        Assert.That(ex.Message, Is.EqualTo("Value cannot be null or whitespace. (Parameter 'hex')"));
    }

    [Test]
    public void Parse_FromHexString_ThrowsArgumentException_WhenHexHasOddNumberOfCharacters()
    {
        // Arrange
        const string hex = "1";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => TlvParser.Parse(hex));
        Assert.That(ex.Message, Is.EqualTo("Value must have an even number of characters. (Parameter 'hex')"));
    }

    [Test]
    public void Parse_FromHexString_ShouldReturnParsedTlv()
    {
        // Arrange
        const string hex = TlvData.HexTlvFirst;

        // Act
        var actual = TlvParser.Parse(hex);

        // Assert
        CheckParsedTlvFirst(actual);
    }

    [Test]
    public void Parse_FromByteArray_ThrowsArgumentException_WhenTlvIsNull()
    {
        // Arrange
        byte[]? tlv = null;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => TlvParser.Parse(tlv!));
        Assert.That(ex.Message, Is.EqualTo("Value must not be null or empty. (Parameter 'tlv')"));
    }

    [Test]
    public void Parse_FromByteArray_ThrowsArgumentException_WhenTlvHasOddNumberOfBytes()
    {
        // Arrange
        var tlv = new byte[] { 0x01 };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => TlvParser.Parse(tlv));
        Assert.That(ex.Message, Is.EqualTo("Value must have an even number of bytes. (Parameter 'tlv')"));
    }
    
    [Test]
    public void Parse_FromByteArray_ShouldReturnParsedTlv()
    {
        // Arrange
        var tlv = TlvData.BytesTlvFirst;

        // Act
        var actual = TlvParser.Parse(tlv);

        // Assert
        CheckParsedTlvFirst(actual);
    }

    private static void CheckParsedTlvFirst(ParsedTlv actual)
    {
        // Arrange
        var expected = TlvData.ExceptedFirst;

        // Assert
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Value, Has.Count.EqualTo(1));
            Assert.That(actual.ToString(), Is.EqualTo(expected.ToString()));
        });

        var rootTlv = actual.Value.First();
        Assert.Multiple(() =>
        {
            Assert.That(rootTlv.Tag, Is.EqualTo(0x6F));
            Assert.That(rootTlv.Length, Is.EqualTo(26));
            Assert.That(rootTlv.IsConstructed, Is.True);
            Assert.That(rootTlv.Data, Is.EqualTo(expected.Value.First().Data));
            Assert.That(rootTlv.Children, Has.Count.EqualTo(2));
        });

        var firstChild = rootTlv.Children.First();
        Assert.Multiple(() =>
        {
            Assert.That(firstChild.Tag, Is.EqualTo(0x84));
            Assert.That(firstChild.Length, Is.EqualTo(14));
            Assert.That(firstChild.IsConstructed, Is.False);
            Assert.That(firstChild.Data, Is.EqualTo(expected.Value.First().Children!.First().Data));
        });

        var secondChild = rootTlv.Children.Last();
        Assert.Multiple(() =>
        {
            Assert.That(secondChild.Tag, Is.EqualTo(0xA5));
            Assert.That(secondChild.Length, Is.EqualTo(8));
            Assert.That(secondChild.IsConstructed, Is.True);
            Assert.That(secondChild.Data, Is.EqualTo(expected.Value.First().Children!.Last().Data));
        });

        Assert.That(secondChild.Children, Has.Count.EqualTo(2));

        var firstChildOfSecondChild = secondChild.Children.First();
        Assert.Multiple(() =>
        {
            Assert.That(firstChildOfSecondChild.Tag, Is.EqualTo(0x88));
            Assert.That(firstChildOfSecondChild.Length, Is.EqualTo(1));
            Assert.That(firstChildOfSecondChild.IsConstructed, Is.False);
            Assert.That(firstChildOfSecondChild.Data,
                Is.EqualTo(expected.Value.First().Children!.Last().Children!.First().Data));
        });

        var secondChildOfSecondChild = secondChild.Children.Last();
        Assert.Multiple(() =>
        {
            Assert.That(secondChildOfSecondChild.Tag, Is.EqualTo(0x5F2D));
            Assert.That(secondChildOfSecondChild.Length, Is.EqualTo(2));
            Assert.That(secondChildOfSecondChild.IsConstructed, Is.False);
            Assert.That(secondChildOfSecondChild.Data,
                Is.EqualTo(expected.Value.First().Children!.Last().Children!.Last().Data));
        });
    }
}