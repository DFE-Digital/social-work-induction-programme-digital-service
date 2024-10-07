using Bogus;
using Dfe.Sww.Ecf.Frontend.TagHelpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using GovUk.Frontend.AspNetCore;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.TagHelpers;

public class SetAccountInactiveLinkTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_WithAccountNameAndHref_GeneratesExpectedOutput()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput(
            "set-account-inactive-link"
        );
        var faker = new Faker();
        var accountName = faker.Person.FullName;
        var href = faker.Internet.UrlRootedPath();
        var sut = new SetAccountInactiveLinkTagHelper() { AccountName = accountName, Href = href };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        var expectedHtml =
            $"<p class=\"govuk-body\">"
            + $"<a class=\"govuk-link govuk-link--no-visited-state\" data-test-id=\"unlink\" href=\"{href}\">{accountName} is leaving this organisation</a>"
            + "<span class=\"govuk-hint govuk-!-display-block\">"
            + "This will unlink this account from this organisation. It will not delete the account or any learner records. "
            + "Coordinators from other organisations will be able to link this account to their own PQP service."
            + "</span>"
            + "</p>";
        output.ToHtmlString().Should().Be(expectedHtml);
    }
}
