namespace MoorTools.Keys.UnitTests;

[TestFixture]
public class KeyGeneratorTests
{
    [Test]
    public void GenerateKey_ReturnsArgumentNullException_WhenKeyLengthIsInvalid()
    {
        // Arrange
        const int invalidKeyLength = 42;

        // Act & Assert
        Assert.That(() => KeyGenerator.GenerateKey((KeyGenerator.KeyLength)invalidKeyLength), Throws.ArgumentException);
    }

    [Test]
    [TestCase(KeyGenerator.KeyLength.Single, 8)]
    [TestCase(KeyGenerator.KeyLength.Double, 16)]
    [TestCase(KeyGenerator.KeyLength.Triple, 24)]
    public void GenerateKey_ReturnsKeyOfCorrectLength(KeyGenerator.KeyLength keyLength, int expectedLength)
    {
        // Act
        var key = KeyGenerator.GenerateKey(keyLength);

        // Assert
        Assert.That(key, Is.Not.Null);
        Assert.That(key, Has.Length.EqualTo(expectedLength));
    }

    [Test]
    public void GenerateKey_ReturnsUniqueKeys()
    {
        // Arrange
        const int keyCount = 10;
        var keys = new List<byte[]>();

        // Act
        for (var i = 0; i < keyCount; i++)
        {
            var key = KeyGenerator.GenerateKey(KeyGenerator.KeyLength.Double);
            keys.Add(key);
        }

        // Assert
        Assert.That(keys, Is.Unique);
    }
}