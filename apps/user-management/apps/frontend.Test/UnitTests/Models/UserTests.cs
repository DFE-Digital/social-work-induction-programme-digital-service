using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Models;

public class UserTests
{
    [Fact]
    public void JsonSerialization_ShouldRetainAllValues()
    {
        var user = new UserBuilder().Build();
        var json = JsonSerializer.Serialize(user);
        var result = JsonSerializer.Deserialize<User>(json);

        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(user);
    }
}
