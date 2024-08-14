using Dfe.Sww.Ecf.Frontend.Models;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Models;

public class SocialWorkEnglandNumberTest
{
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Now);

    [Fact]
    public void WhenSweNumberNegativeSingleParam_ThrowException()
    {
        //Arrange & Act
        Action action = () => _ = new SocialWorkEnglandNumber(-1);

        //Assert
        action
            .Should()
            .Throw<ArgumentException>()
            .WithMessage("Social Work England number must not be negative");
    }

    [Fact]
    public void WhenSweNumberNegative_ThrowException()
    {
        //Arrange & Act
        Action action = () => _ = new SocialWorkEnglandNumber(-1, _today);

        //Assert
        action
            .Should()
            .Throw<ArgumentException>()
            .WithMessage("Social Work England number must not be negative");
    }

    [Fact]
    public void WhenDueDateInFuture_RegisteredCheckRequireFalse()
    {
        //Arrange
        var account = new SocialWorkEnglandNumber(new Random().Next(), _today.AddDays(1));

        //Act
        var result = account.IsStatusCheckRequired();

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void WhenCheckIsDue_RegisteredCheckRequireTrue()
    {
        //Arrange
        var account = new SocialWorkEnglandNumber(new Random().Next(), _today);

        //Act
        var result = account.IsStatusCheckRequired();

        //Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("SW1234")]
    [InlineData("1234")]
    public void ExtractNumber_ReturnsInt(string sweId)
    {
        //Arrange
        var expectedObject = new SocialWorkEnglandNumber(1234);

        //Act
        var result = SocialWorkEnglandNumber.Parse(sweId);

        //Assert
        result.Should().BeEquivalentTo(expectedObject);
    }

    [Fact]
    public void WhenArgumentIsNull_EqualsReturnsFalse()
    {
        //Arrange
        var account = new SocialWorkEnglandNumber(new Random().Next(), _today);

        //Act
        var result = account.Equals(null);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void WhenObjectIsNotSocialWorkEnglandNumber_EqualsReturnsFalse()
    {
        //Arrange
        var account = new SocialWorkEnglandNumber(new Random().Next(), _today);
        var obj = new object();

        //Act
        var result = account.Equals(obj);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void WhenObjectsHoldTheSameNumber_EqualsReturnsTrue()
    {
        //Arrange
        var account1 = new SocialWorkEnglandNumber(1234, _today);
        var account2 = new SocialWorkEnglandNumber(1234, _today.AddDays(1));

        //Act
        var result = account1.Equals(account2);

        //Assert
        result.Should().BeTrue();
    }
}
