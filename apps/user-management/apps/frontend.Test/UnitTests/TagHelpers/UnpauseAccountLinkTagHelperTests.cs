using Bogus;
using Dfe.Sww.Ecf.Frontend.TagHelpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using GovUk.Frontend.AspNetCore;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.TagHelpers;

public class UnpauseAccountLinkTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_WithAccountNameAndHref_GeneratesExpectedOutput()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("unpause-account-link");
        var faker = new Faker();
        var accountName = faker.Person.FullName;
        var href = faker.Internet.UrlRootedPath();
        var sut = new UnpauseAccountLinkTagHelper() { AccountName = accountName, Href = href };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        var expectedHtml =
            $"<p class=\"govuk-body\">"
            + $"<a class=\"govuk-link govuk-link--no-visited-state\" href=\"{href}\">{accountName} is returning to the PQP programme</a>"
            + "<span class=\"govuk-hint govuk-!-display-block\">"
            + "This will unpause this account. "
            + $"{accountName} will have full access to the digital service."
            + "</span>"
            + "</p>";
        output.ToHtmlString().Should().Be(expectedHtml);
    }
}
