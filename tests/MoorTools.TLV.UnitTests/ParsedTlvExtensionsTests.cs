using MoorTools.TLV.Extensions;
using MoorTools.TLV.Models;

namespace MoorTools.TLV.UnitTests;

[TestFixture]
public class ParsedTlvExtensionsTests
{
    [Test]
    public void FindTag_ReturnsEmptyList_WhenNoMatchingTagsFound()
    {
        // Arrange
        var parsedTlv = new ParsedTlv([
            new Tlv
            {
                Tag = 1
            },
            new Tlv
            {
                Tag = 2
            },
            new Tlv
            {
                Tag = 3
            }
        ]);

        // Act
        var result = parsedTlv.FindTag(4);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void FindTag_ShouldReturnMatchingTags_WhenTagsExist()
    {
        // Arrange
        var parsedTlv = new ParsedTlv([
            new Tlv
            {
                Tag = 1
            },
            new Tlv
            {
                Tag = 2
            },
            new Tlv
            {
                Tag = 1
            }
        ]);

        // Act
        var result = parsedTlv.FindTag(1);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.TrueForAll(tlv => tlv.Tag == 1), Is.True);
    }

    [Test]
    public void FindTag_ShouldFindTagsRecursively_WhenConstructedTlvHasChildren()
    {
        // Arrange
        var parsedTlv = new ParsedTlv([
            new Tlv
            {
                Tag = 1,
                IsConstructed = true,
                Children =
                [
                    new Tlv
                    {
                        Tag = 1
                    },
                    new Tlv
                    {
                        Tag = 2
                    }
                ]
            },
            new Tlv
            {
                Tag = 3
            }
        ]);

        // Act
        var result = parsedTlv.FindTag(1);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.TrueForAll(tlv => tlv.Tag == 1), Is.True);
    }

    [Test]
    public void FindTag_ShouldReturnEmptyList_WhenParsedTlvHasNoChildren()
    {
        // Arrange
        var parsedTlv = new ParsedTlv([]);

        // Act
        var result = parsedTlv.FindTag(1);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void FindTag_ShouldReturnEmptyList_WhenChildrenIsNull()
    {
        // Arrange
        var parsedTlv = new ParsedTlv([
            new Tlv
            {
                Tag = 1,
                IsConstructed = true,
                Children = null
            }
        ]);

        // Act
        var result = parsedTlv.FindTag(1);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
    }
}