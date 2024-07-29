using Bogus;
using Dfe.Sww.Ecf.Frontend.TagHelpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using GovUk.Frontend.AspNetCore;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.TagHelpers;

public class PauseAccountLinkTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_WithAccountNameAndHref_GeneratesExpectedOutput()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("pause-account-link");
        var faker = new Faker();
        var accountName = faker.Person.FullName;
        var href = faker.Internet.UrlRootedPath();
        var sut = new PauseAccountLinkTagHelper() { AccountName = accountName, Href = href };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        var expectedHtml =
            $"<p class=\"govuk-body\">"
            + $"<a class=\"govuk-link govuk-link--no-visited-state\" href=\"{href}\">{accountName} is taking a break from the PQP programme</a>"
            + "<span class=\"govuk-hint govuk-!-display-block\">"
            + "This will temporarily pause this account while the staff member is taking a break from the programme (for example, parental leave or other leave of absence). "
            + "It will not delete the account or any learner records."
            + "</span>"
            + "</p>";
        output.ToHtmlString().Should().Be(expectedHtml);
    }
}
