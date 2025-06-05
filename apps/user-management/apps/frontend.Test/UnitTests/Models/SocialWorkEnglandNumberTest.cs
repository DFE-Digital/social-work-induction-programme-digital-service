using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Models;

public class SocialWorkEnglandRecordTest
{
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);

    [Fact]
    public void WhenSweNumberNegativeSingleParam_ThrowException()
    {
        // Arrange & Act
        Action action = () => _ = new SocialWorkEnglandRecord(-1);

        // Assert
        action
            .Should()
            .Throw<ArgumentException>()
            .WithMessage("Social Work England number must not be negative");
    }

    [Fact]
    public void WhenSweNumberNegative_ThrowException()
    {
        // Arrange & Act
        Action action = () => _ = new SocialWorkEnglandRecord(-1, _today);

        // Assert
        action
            .Should()
            .Throw<ArgumentException>()
            .WithMessage("Social Work England number must not be negative");
    }

    [Fact]
    public void WhenDueDateInFuture_RegisteredCheckRequireFalse()
    {
        // Arrange
        var sweRecord = new SocialWorkEnglandRecord(new Random().Next(), _today.AddDays(1));

        // Act
        var result = sweRecord.IsStatusCheckRequired();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void WhenCheckIsDue_RegisteredCheckRequireTrue()
    {
        // Arrange
        var sweRecord = new SocialWorkEnglandRecord(new Random().Next(), _today);

        // Act
        var result = sweRecord.IsStatusCheckRequired();

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("SW1234")]
    [InlineData("1234")]
    public void ExtractNumber_ReturnsInt(string sweId)
    {
        // Arrange
        var expectedObject = new SocialWorkEnglandRecord(1234);

        // Act
        var result = SocialWorkEnglandRecord.Parse(sweId);

        // Assert
        result.Should().BeEquivalentTo(expectedObject);
    }

    [Theory]
    [InlineData("SW1234")]
    [InlineData("sw1234")]
    [InlineData("1234")]
    public void TryParse_WithValidInputs_ReturnsSocialWorkEnglandNumber(string sweId)
    {
        // Arrange
        var expectedObject = new SocialWorkEnglandRecord(1234);

        // Act
        var result = SocialWorkEnglandRecord.TryParse(sweId, out var sweNumber);

        // Assert
        result.Should().BeTrue();
        sweNumber.Should().BeEquivalentTo(expectedObject);
    }

    [Theory]
    [InlineData("SWE123")]
    [InlineData("SW123456789123456789")]
    [InlineData("123456789123456789")]
    [InlineData("SW0123")]
    [InlineData("SW1234567")]
    [InlineData("1234567")]
    [InlineData("-500")]
    [InlineData("0")]
    [InlineData("")]
    public void TryParse_WithInvalidInputs_ReturnsSocialWorkEnglandNumber(string sweId)
    {
        // Arrange & Act
        var result = SocialWorkEnglandRecord.TryParse(sweId, out var sweNumber);

        // Assert
        result.Should().BeFalse();
        sweNumber.Should().BeNull();
    }

    [Fact]
    public void WhenArgumentIsNull_EqualsReturnsFalse()
    {
        // Arrange
        var sweRecord = new SocialWorkEnglandRecord(new Random().Next(), _today);

        // Act
        var result = sweRecord.Equals(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void WhenObjectIsNotSocialWorkEnglandNumber_EqualsReturnsFalse()
    {
        // Arrange
        var sweRecord = new SocialWorkEnglandRecord(new Random().Next(), _today);
        var obj = new object();

        // Act
        var result = sweRecord.Equals(obj);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void WhenObjectsHoldTheSameNumber_EqualsReturnsTrue()
    {
        // Arrange
        var sweRecord1 = new SocialWorkEnglandRecord(1234, _today);
        var sweRecord2 = new SocialWorkEnglandRecord(1234, _today.AddDays(1));

        // Act
        var result = sweRecord1.Equals(sweRecord2);

        // Assert
        result.Should().BeTrue();
    }
}
