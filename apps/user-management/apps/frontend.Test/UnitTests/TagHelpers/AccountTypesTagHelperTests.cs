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
        output.ToHtmlString(HtmlEncoder.Default).Should().Be(sut.Types[0].GetDisplayName());
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
            .ToHtmlString(HtmlEncoder.Default)
            .Should()
            .Be($"<p class=\"govuk-body\">{sut.Types[0].GetDisplayName()}</p><p class=\"govuk-body\">{sut.Types[1].GetDisplayName()}</p>");
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
        output.ToHtmlString(HtmlEncoder.Default).Should().Be(string.Empty);
    }
}
