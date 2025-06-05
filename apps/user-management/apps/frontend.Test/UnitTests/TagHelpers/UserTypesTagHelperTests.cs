using System.Text.Encodings.Web;
using Bogus;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.TagHelpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using GovUk.Frontend.AspNetCore;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.TagHelpers;

public class UserTypesTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_WithSingleType_GeneratesExpectedOutput()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-types");
        var sut = new UserTypesTagHelper { Types = [new Faker().PickRandom<UserType>()] };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        output.ToHtmlString(HtmlEncoder.Default).Should().Be(sut.Types[0].GetDisplayName());
    }

    [Fact]
    public async Task ProcessAsync_WithMultipleTypes_GeneratesExpectedOutput()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-types");
        var sut = new UserTypesTagHelper
        {
            Types = [new Faker().PickRandom<UserType>(), new Faker().PickRandom<UserType>()]
        };
        await sut.ProcessAsync(context, output);

        // Assert
        output
            .ToHtmlString(HtmlEncoder.Default)
            .Should()
            .Be($"{sut.Types[0].GetDisplayName()}, {sut.Types[1].GetDisplayName()}");
    }

    [Fact]
    public async Task ProcessAsync_WithNoTypes_GeneratesExpectedOutput()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-types");
        var sut = new UserTypesTagHelper { Types = null };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        output.ToHtmlString(HtmlEncoder.Default).Should().Be(string.Empty);
    }
}
