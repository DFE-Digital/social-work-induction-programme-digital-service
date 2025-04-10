using System.Text.Json;
using SocialWorkInductionProgramme.Frontend.Models;
using SocialWorkInductionProgramme.Frontend.Test.UnitTests.Helpers.Builders;
using FluentAssertions;
using Xunit;

namespace SocialWorkInductionProgramme.Frontend.Test.UnitTests.Models;

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
