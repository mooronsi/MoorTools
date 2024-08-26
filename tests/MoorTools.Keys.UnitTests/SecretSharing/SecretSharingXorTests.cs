using SecretSharingXor = MoorTools.Keys.SecretSharing.Xor;

namespace MoorTools.Keys.UnitTests.SecretSharing;

[TestFixture]
public class SecretSharingXorTests
{
    [Test]
    public void CombineKey_ThrowsArgumentException_WhenPartsIsEmpty()
    {
        // Arrange
        const int partCount = 2;
        var parts = Array.Empty<byte[]>();

        // Act & Assert
        Assert.That(() => SecretSharingXor.CombineKey(partCount, parts), Throws.ArgumentException);
    }

    [Test]
    public void CombineKey_ThrowsArgumentException_WhenPartsHaveDifferentLengths()
    {
        // Arrange
        const int partCount = 2;
        var parts = new[]
        {
            new byte[] { 0x01, 0x02, 0x03 },
            [0x04, 0x05]
        };

        // Act & Assert
        Assert.That(() => SecretSharingXor.CombineKey(partCount, parts), Throws.ArgumentException);
    }

    [Test]
    public void CombineKey_ReturnsCorrectKeyLength()
    {
        // Arrange
        const int partCount = 2;
        var parts = new[]
        {
            new byte[] { 0x01, 0x02, 0x03 },
            [0x04, 0x05, 0x06]
        };

        // Act
        var key = SecretSharingXor.CombineKey(partCount, parts);

        // Assert
        Assert.That(key, Has.Length.EqualTo(parts[0].Length));
    }

    [Test]
    [TestCase(1, "6843E36D076D4355", "6843E36D076D4355")]
    [TestCase(2, "6843E36D076D4355", "484B71DC11EF95C1", "200892B11682D694")]
    [TestCase(3, "6843E36D076D4355", "79BC1D381EC625F1", "5D712149172EE544", "4C8EDF1C0E8583E0")]
    [TestCase(4, "6843E36D076D4355", "7A012A1DD4EA3130", "98527D7E56166BED", "AD2F49EE439F5F0E",
        "273FFDE0C60E4686")]
    [TestCase(1, "A32419EF1827C6630A4BE047D59368CC", "A32419EF1827C6630A4BE047D59368CC")]
    [TestCase(2, "A32419EF1827C6630A4BE047D59368CC", "4217D1A1A9240484D4265BDB1CBA2932",
        "E133C84EB103C2E7DE6DBB9CC92941FE")]
    [TestCase(3, "A32419EF1827C6630A4BE047D59368CC", "6271E699295AC7919792A5ECCFD884CD",
        "F3BDA269F75613219320CC96EF730DB0", "32E85D1FC62B12D30EF9893DF538E1B1")]
    [TestCase(4, "A32419EF1827C6630A4BE047D59368CC", "20499C379B9A7044AF57730D7E281E92",
        "A8B23811F6D908AAACE4551DB6C5FDF8", "E7F3A992A5824D2A5F95DB0A43C53996", "CC2C145BD0E6F3A7566D1D5D5EBBB230")]
    [TestCase(1, "3B916B9E575538FF70D6439BFF1E8EF04E3C41369689FCD3",
        "3B916B9E575538FF70D6439BFF1E8EF04E3C41369689FCD3")]
    [TestCase(2, "3B916B9E575538FF70D6439BFF1E8EF04E3C41369689FCD3",
        "AEED2B50491DCB3715B6C2E14D6F7BDBEC5102375DD25541", "957C40CE1E48F3C86560817AB271F52BA26D4301CB5BA992")]
    [TestCase(3, "3B916B9E575538FF70D6439BFF1E8EF04E3C41369689FCD3",
        "CC1453A009D4E1FCF51A3F626631B1C9E3773859342B5C24", "07F7066EB4CA2D10FAFA11CC5B03CACA10D52CBA60F96007",
        "F0723E50EA4BF4137F366D35C22CF5F3BD9E55D5C25BC0F0")]
    [TestCase(4, "3B916B9E575538FF70D6439BFF1E8EF04E3C41369689FCD3",
        "2D6A1D7300D797A674E78E3DEFB679CE4AE9416EFCCCB464",
        "61DCF602AD74115271123070EFA0ECD6DFDEC138AE4EE80C", "250CBADA74F756FFB10084F515C644BC84CF8CDF1033EE58",
        "522B3A358E01E8F4C4237923EACE5F545FC44DBFD4384EE3")]
    public void CombineKey_ReturnsCorrectKey(int partCount, string key, params string[] parts)
    {
        // Arrange
        var keyBytes = Convert.FromHexString(key);
        var partBytes = parts.Select(Convert.FromHexString).ToArray();

        // Act
        var combinedKey = SecretSharingXor.CombineKey(partCount, partBytes);

        // Assert
        Assert.That(combinedKey, Is.EqualTo(keyBytes));
    }

    [Test]
    public void SplitKey_ThrowsArgumentException_WhenPartCountIsLessThanTwo()
    {
        // Arrange
        const int partCount = 1;
        var key = new byte[] { 0x01, 0x02, 0x03, 0x04 };

        // Act & Assert
        Assert.That(() => SecretSharingXor.SplitKey(partCount, key), Throws.ArgumentException);
    }

    [Test]
    public void SplitKey_ThrowsArgumentException_WhenKeyIsEmpty()
    {
        // Arrange
        const int partCount = 2;
        var key = Array.Empty<byte>();

        // Act & Assert
        Assert.That(() => SecretSharingXor.SplitKey(partCount, key), Throws.ArgumentException);
    }

    [Test]
    public void SplitKey_ReturnsCorrectNumberOfParts_WhenKeyIsValid()
    {
        // Arrange
        const int partCount = 3;
        var key = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };

        // Act
        var parts = SecretSharingXor.SplitKey(partCount, key);

        // Assert
        Assert.That(parts, Has.Length.EqualTo(partCount));
    }

    [Test]
    public void SplitKey_PartsHaveSameLengthAsOriginalKey()
    {
        // Arrange
        const int partCount = 3;
        var key = new byte[] { 1, 2, 3, 4 };

        // Act
        var parts = SecretSharingXor.SplitKey(partCount, key);

        // Assert
        Assert.That(parts.All(part => part.Length == key.Length), Is.True);
    }

    [Test]
    public void SplitKey_And_CombineKey_ShouldReconstructOriginalKey()
    {
        // Arrange
        var originalKey = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        const int partCount = 3;

        // Act
        var parts = SecretSharingXor.SplitKey(partCount, originalKey);
        var reconstructedKey = SecretSharingXor.CombineKey(partCount, parts);

        // Assert
        Assert.That(reconstructedKey, Is.EqualTo(originalKey));
    }
}