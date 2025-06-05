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

public class UserDetailsTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_WithSocialWorkerUser_GeneratesExpectedOutput()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-details");
        var sut = new UserDetailsTagHelper
        {
            User = new UserBuilder()
                .WithTypes(ImmutableList.Create(UserType.EarlyCareerSocialWorker))
                .WithSocialWorkEnglandNumber()
                .Build()
        };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        var expectedHtml =
            $"<p>{sut.User.FullName}</p>"
            + $"<p>{sut.User.Email}</p>"
            + $"<p>SWE registration number {sut.User.SocialWorkEnglandNumber}</p>";

        output.ToHtmlString(HtmlEncoder.Default).Should().Be(expectedHtml);
    }

    [Fact]
    public async Task ProcessAsync_WithSocialWorkerWithNoSweNumberUser_DisplaysMissingSweNumberMessage()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-details");
        var sut = new UserDetailsTagHelper()
        {
            User = new UserBuilder()
                .WithTypes(ImmutableList.Create(UserType.EarlyCareerSocialWorker))
                .WithSocialWorkEnglandNumber(null)
                .WithStatus(UserStatus.PendingRegistration)
                .Build()
        };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        var expectedHtml =
            $"<p>{sut.User.FullName}</p>"
            + $"<p>{sut.User.Email}</p>"
            + "<p>"
            + $"<govuk-tag class=\"govuk-tag govuk-tag--orange\">{UserStatus.PendingRegistration.GetDisplayName()}</govuk-tag>"
            + "<span class=\"govuk-!-display-block govuk-hint govuk-!-margin-bottom-0\">"
            + "You have not provided a Social Work England registration number for this account"
            + "</span>"
            + "</p>";

        output.ToHtmlString(HtmlEncoder.Default).Should().Be(expectedHtml);
    }

    [Theory]
    [InlineData(UserType.Coordinator)]
    [InlineData(UserType.Assessor)]
    [InlineData(UserType.Assessor, UserType.Coordinator)]
    public async Task ProcessAsync_WithNonSocialWorkerUser_DoesNotIncludeSweNumber(
        params UserType[] accountTypes
    )
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-details");
        var sut = new UserDetailsTagHelper()
        {
            User = new UserBuilder().WithTypes(ImmutableList.Create(accountTypes)).Build()
        };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        var expectedHtml = $"<p>{sut.User.FullName}</p><p>{sut.User.Email}</p>";

        output.ToHtmlString(HtmlEncoder.Default).Should().Be(expectedHtml);
    }

    [Fact]
    public async Task ProcessAsync_WithNoUser_DoesNotIncludeDetails()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("account-details");
        var sut = new UserDetailsTagHelper();

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        output.ToHtmlString(HtmlEncoder.Default).Should().BeEmpty();
    }
}
