using System.Collections.Immutable;
using System.Text.Encodings.Web;
using Dfe.Sww.Ecf.Frontend.Extensions;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.TagHelpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers.Builders;
using FluentAssertions;
using GovUk.Frontend.AspNetCore;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.TagHelpers;

public class AccountDetailsTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_WithSocialWorkerAccount_GeneratesExpectedOutput()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-details");
        var sut = new AccountDetailsTagHelper
        {
            Account = new AccountBuilder()
                .WithTypes(ImmutableList.Create(AccountType.EarlyCareerSocialWorker))
                .WithSocialWorkEnglandNumber()
                .Build()
        };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        var expectedHtml =
            $"<p>{sut.Account.FullName}</p>"
            + $"<p>{sut.Account.Email}</p>"
            + $"<p>SWE registration number {sut.Account.SocialWorkEnglandNumber}</p>";

        output.ToHtmlString(HtmlEncoder.Default).Should().Be(expectedHtml);
    }

    [Fact]
    public async Task ProcessAsync_WithSocialWorkerWithNoSweNumberAccount_DisplaysMissingSweNumberMessage()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-details");
        var sut = new AccountDetailsTagHelper()
        {
            Account = new AccountBuilder()
                .WithTypes(ImmutableList.Create(AccountType.EarlyCareerSocialWorker))
                .WithSocialWorkEnglandNumber(null)
                .WithStatus(AccountStatus.PendingRegistration)
                .Build()
        };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        var expectedHtml =
            $"<p>{sut.Account.FullName}</p>"
            + $"<p>{sut.Account.Email}</p>"
            + "<p>"
            + $"<govuk-tag class=\"govuk-tag govuk-tag--orange\">{AccountStatus.PendingRegistration.GetDisplayName()}</govuk-tag>"
            + "<span class=\"govuk-!-display-block govuk-hint govuk-!-margin-bottom-0\">"
            + "You have not provided a Social Work England registration number for this account"
            + "</span>"
            + "</p>";

        output.ToHtmlString(HtmlEncoder.Default).Should().Be(expectedHtml);
    }

    [Theory]
    [InlineData(AccountType.Coordinator)]
    [InlineData(AccountType.Assessor)]
    [InlineData(AccountType.Assessor, AccountType.Coordinator)]
    public async Task ProcessAsync_WithNonSocialWorkerAccount_DoesNotIncludeSweNumber(
        params AccountType[] accountTypes
    )
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-details");
        var sut = new AccountDetailsTagHelper()
        {
            Account = new AccountBuilder().WithTypes(ImmutableList.Create(accountTypes)).Build()
        };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        var expectedHtml = $"<p>{sut.Account.FullName}</p><p>{sut.Account.Email}</p>";

        output.ToHtmlString(HtmlEncoder.Default).Should().Be(expectedHtml);
    }

    [Fact]
    public async Task ProcessAsync_WithNoAccount_DoesNotIncludeDetails()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-details");
        var sut = new AccountDetailsTagHelper();

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        output.ToHtmlString(HtmlEncoder.Default).Should().BeEmpty();
    }
}
