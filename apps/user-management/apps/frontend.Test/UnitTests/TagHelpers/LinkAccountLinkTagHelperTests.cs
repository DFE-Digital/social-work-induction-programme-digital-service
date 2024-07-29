using Bogus;
using Dfe.Sww.Ecf.Frontend.TagHelpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using GovUk.Frontend.AspNetCore;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.TagHelpers;

public class LinkAccountLinkTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_WithAccountNameAndHref_GeneratesExpectedOutput()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("link-account-link");
        var faker = new Faker();
        var accountName = faker.Person.FullName;
        var href = faker.Internet.UrlRootedPath();
        var sut = new LinkAccountLinkTagHelper { AccountName = accountName, Href = href };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        var expectedHtml =
            $"<p class=\"govuk-body\"><a class=\"govuk-link govuk-link--no-visited-state\" href=\"{href}\">Link {accountName} to this organisation</a></p>";
        output.ToHtmlString().Should().Be(expectedHtml);
    }
}
