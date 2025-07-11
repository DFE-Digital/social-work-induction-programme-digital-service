using System.Collections.Immutable;
using System.Text.Encodings.Web;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.Models.ManageOrganisations;
using Dfe.Sww.Ecf.Frontend.TagHelpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using FluentAssertions;
using GovUk.Frontend.AspNetCore;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.TagHelpers;

public class GovUkCardsTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_WithCardItem_GeneratesExpectedOutput()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("govuk-cards");
        var sut = new GovUkCardsTagHelper()
        {
            Heading = "Dashboard",
            SubHeadingLevel = 3,
            Items = new List<CardItem>
            {
                new()
                {
                    Link = new CardLink
                    {
                        Text = "Manage organisations",
                        Path = "/manage-organisations",
                        Attributes = new Dictionary<string, string> { ["aria-label"] = "link to manage organisations page" }
                    },
                    Description = "Add or edit organisations and manage users."
                }
            },
            Columns = 0
        };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        var expectedHtml =
            "<div class=\"gem-c-cards\">"
            + "<h1 class=\"gem-c-cards__heading govuk-heading-l gem-c-cards__heading--underline\">Dashboard</h1>"
            + "<ul class=\"gem-c-cards__list gem-c-cards__list--one-column\">"
            + "<li class=\"gem-c-cards__list-item\">"
            + "<div class=\"gem-c-cards__list-item-wrapper\">"
            + "<h3 class=\"gem-c-cards__sub-heading govuk-heading-s\">"
            + "<a aria-label=\"link to manage organisations page\" class=\"govuk-link gem-c-cards__link gem-c-force-print-link-styles\" href=\"/manage-organisations\">Manage organisations</a>"
            + "<p class=\"govuk-body gem-c-cards__description\">Add or edit organisations and manage users.</p>"
            + "</h3></div></li></ul></div>";

        output.ToHtmlString(HtmlEncoder.Default).Should().Be(expectedHtml);
    }

    [Fact]
    public async Task ProcessAsync_WithNoItems_ThrowsException()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-details");
        var sut = new GovUkCardsTagHelper();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await sut.ProcessAsync(context, output)
        );
        Assert.Equal("Items", ex.ParamName);
        Assert.Contains("The cards component requires an Items collection.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_WithNoLink_ThrowsException()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-details");
        var sut = new GovUkCardsTagHelper()
        {
            Heading = "Dashboard",
            SubHeadingLevel = 3,
            Items = new List<CardItem>
            {
                new()
                {
                    Link = null,
                    Description = "test description"
                }
            },
            Columns = 0
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(
            async () => await sut.ProcessAsync(context, output)
        );
        Assert.Contains("Each CardItem must have a link with a non-empty path.", ex.Message);
    }

    [Fact]
    public async Task ProcessAsync_WithItemDescriptionEmpty_DoesNotIncludeParagraph()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("govuk-cards");
        var sut = new GovUkCardsTagHelper()
        {
            Heading = "Dashboard",
            SubHeadingLevel = 3,
            Items = new List<CardItem>
            {
                new()
                {
                    Link = new CardLink
                    {
                        Text = "Manage organisations",
                        Path = "/manage-organisations",
                        Attributes = new Dictionary<string, string> { ["aria-label"] = "link to manage organisations page" }
                    }
                }
            },
            Columns = 0
        };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        var expectedHtml =
            "<div class=\"gem-c-cards\">"
            + "<h1 class=\"gem-c-cards__heading govuk-heading-l gem-c-cards__heading--underline\">Dashboard</h1>"
            + "<ul class=\"gem-c-cards__list gem-c-cards__list--one-column\">"
            + "<li class=\"gem-c-cards__list-item\">"
            + "<div class=\"gem-c-cards__list-item-wrapper\">"
            + "<h3 class=\"gem-c-cards__sub-heading govuk-heading-s\">"
            + "<a aria-label=\"link to manage organisations page\" class=\"govuk-link gem-c-cards__link gem-c-force-print-link-styles\" href=\"/manage-organisations\">Manage organisations</a>"
            + "</h3></div></li></ul></div>";

        output.ToHtmlString(HtmlEncoder.Default).Should().Be(expectedHtml);
    }
}
