using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using FluentAssertions;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Models;

public class AccountTests
{
    [Fact]
    public void JsonSerialization_ShouldRetainAllValues()
    {
        var account = new AccountBuilder().Build();
        var json = JsonSerializer.Serialize(account);
        var result = JsonSerializer.Deserialize<Account>(json);

        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(account);
    }
}
