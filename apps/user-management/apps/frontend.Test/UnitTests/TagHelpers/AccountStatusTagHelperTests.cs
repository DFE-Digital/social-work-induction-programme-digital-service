using System.Text.Encodings.Web;
using Dfe.Sww.Ecf.Frontend.Models;
using Dfe.Sww.Ecf.Frontend.TagHelpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using GovUk.Frontend.AspNetCore;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.TagHelpers;

public class AccountStatusTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_WithActiveStatus_GeneratesExpectedOutput()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-status");
        var sut = new AccountStatusTagHelper { Status = AccountStatus.Active };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        const string expectedHtml =
            "<strong class=\"govuk-tag govuk-tag--green\" data-test-class=\"account-status\">Active</strong>";
        output.ToHtmlString(HtmlEncoder.Default).Should().Be(expectedHtml);
    }

    [Fact]
    public async Task ProcessAsync_WithPendingStatus_GeneratesExpectedOutput()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-status");
        var sut = new AccountStatusTagHelper { Status = AccountStatus.PendingRegistration };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        const string expectedHtml =
            "<strong class=\"govuk-tag govuk-tag--grey\" data-test-class=\"account-status\">Pending</strong>";
        output.ToHtmlString(HtmlEncoder.Default).Should().Be(expectedHtml);
    }

    [Fact]
    public async Task ProcessAsync_WithInactiveStatus_GeneratesExpectedOutput()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-status");
        var sut = new AccountStatusTagHelper { Status = AccountStatus.Inactive };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        const string expectedHtml =
            "<strong class=\"govuk-tag govuk-tag--yellow\" data-test-class=\"account-status\">Inactive</strong>";
        output.ToHtmlString(HtmlEncoder.Default).Should().Be(expectedHtml);
    }

    [Fact]
    public async Task ProcessAsync_WithBlankStatus_GeneratesExpectedOutput()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-status");
        var sut = new AccountStatusTagHelper { Status = null };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        const string expectedHtml = "";
        output.ToHtmlString(HtmlEncoder.Default).Should().Be(expectedHtml);
    }
}
