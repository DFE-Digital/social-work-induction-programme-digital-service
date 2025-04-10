using Bogus;
using SocialWorkInductionProgramme.Frontend.Extensions;
using SocialWorkInductionProgramme.Frontend.Models;
using SocialWorkInductionProgramme.Frontend.TagHelpers;
using SocialWorkInductionProgramme.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using GovUk.Frontend.AspNetCore;
using Xunit;

namespace SocialWorkInductionProgramme.Frontend.Test.UnitTests.TagHelpers;

public class AccountTypesTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_WithSingleType_GeneratesExpectedOutput()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-types");
        var sut = new AccountTypesTagHelper { Types = [new Faker().PickRandom<AccountType>()] };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        output.ToHtmlString().Should().Be(sut.Types[0].GetDisplayName());
    }

    [Fact]
    public async Task ProcessAsync_WithMultipleTypes_GeneratesExpectedOutput()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-types");
        var sut = new AccountTypesTagHelper
        {
            Types = [new Faker().PickRandom<AccountType>(), new Faker().PickRandom<AccountType>()]
        };
        await sut.ProcessAsync(context, output);

        // Assert
        output
            .ToHtmlString()
            .Should()
            .Be($"{sut.Types[0].GetDisplayName()}, {sut.Types[1].GetDisplayName()}");
    }

    [Fact]
    public async Task ProcessAsync_WithNoTypes_GeneratesExpectedOutput()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-types");
        var sut = new AccountTypesTagHelper { Types = null };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        output.ToHtmlString().Should().Be(string.Empty);
    }
}
