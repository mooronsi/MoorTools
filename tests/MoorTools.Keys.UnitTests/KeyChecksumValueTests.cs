namespace MoorTools.Keys.UnitTests;

[TestFixture]
public class KeyChecksumValueTests
{
    [Test]
    [TestCase("CFC46B6B23196D85B5162F05465064BB", "38B7DF")]
    [TestCase("3D4585932F5922CEF93E729083B812E798C1F34C9113BD40", "FAAFB1")]
    public void Calc_ReturnsCorrectKcv(string keyHex, string expectedKcvHex)
    {
        // Arrange
        var key = Convert.FromHexString(keyHex);
        var expectedKcv = Convert.FromHexString(expectedKcvHex);

        // Act
        var kcv = KeyChecksumValue.Calc(key);

        // Assert
        Assert.That(kcv, Is.EqualTo(expectedKcv));
    }

    [Test]
    public void Calc_ReturnsArgumentException_WhenKeyLengthIsInvalid()
    {
        // Arrange
        var key = new byte[17];

        // Act & Assert
        Assert.That(() => KeyChecksumValue.Calc(key), Throws.ArgumentException);
    }

    [Test]
    public void Calc_ReturnsDifferentKcv_WhenKeysAreDifferent()
    {
        // Arrange
        var key1 = KeyGenerator.GenerateKey(KeyGenerator.KeyLength.Double);
        var key2 = KeyGenerator.GenerateKey(KeyGenerator.KeyLength.Double);

        // Act
        var kcv1 = KeyChecksumValue.Calc(key1);
        var kcv2 = KeyChecksumValue.Calc(key2);

        // Assert
        Assert.That(kcv1, Is.Not.EqualTo(kcv2));
    }

    [Test]
    public void Calc_ReturnsSameKcv_WhenKeysAreEqual()
    {
        // Arrange
        var key = KeyGenerator.GenerateKey(KeyGenerator.KeyLength.Double);

        // Act
        var kcv1 = KeyChecksumValue.Calc(key);
        var kcv2 = KeyChecksumValue.Calc(key);

        // Assert
        Assert.That(kcv1, Is.EqualTo(kcv2));
    }
}