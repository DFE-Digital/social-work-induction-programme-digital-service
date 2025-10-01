using System.Net;
using FluentAssertions;

namespace Dfe.Sww.Ecf.AuthorizeAccess.Tests.PageTests;

[Collection("Uses Database")]
public class NotFoundTests(HostFixture hostFixture) : TestBase(hostFixture)
{
    [Fact]
    public async Task Get_NotAuthenticatedWithOneLogin_ReturnsBadRequest()
    {
        // Arrange
        var state = CreateNewState();
        var journeyInstance = await CreateJourneyInstance(state);

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/not-found?{journeyInstance.GetUniqueIdQueryParameter()}"
        );

        // Act
        var response = await HttpClient.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);
    }

    [Fact]
    public async Task Get_NotVerifiedWithOneLogin_ReturnsBadRequest()
    {
        // Arrange
        var state = CreateNewState();
        var journeyInstance = await CreateJourneyInstance(state);

        var oneLoginUser = await TestData.CreateOneLoginUser(verified: false);

        var ticket = CreateOneLoginAuthenticationTicket(
            SignInJourneyHelper.AuthenticationOnlyVtr,
            oneLoginUser
        );
        await GetSignInJourneyHelper().OnOneLoginCallback(journeyInstance, ticket);

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/not-found?{journeyInstance.GetUniqueIdQueryParameter()}"
        );

        // Act
        var response = await HttpClient.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);
    }

    [Fact]
    public async Task Get_AlreadyAuthenticated_RedirectsToStateRedirectUri()
    {
        // Arrange
        var state = CreateNewState();
        var journeyInstance = await CreateJourneyInstance(state);

        var person = await TestData.CreatePerson(b => b.WithTrn().WithNationalInsuranceNumber());
        var oneLoginUser = await TestData.CreateOneLoginUser(person);

        var ticket = CreateOneLoginAuthenticationTicket(
            SignInJourneyHelper.AuthenticationOnlyVtr,
            oneLoginUser
        );
        await GetSignInJourneyHelper().OnOneLoginCallback(journeyInstance, ticket);

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/not-found?{journeyInstance.GetUniqueIdQueryParameter()}"
        );

        // Act
        var response = await HttpClient.SendAsync(request);

        // Assert
        Assert.Equal(StatusCodes.Status302Found, (int)response.StatusCode);
        Assert.Equal(
            $"{state.RedirectUri}?{journeyInstance.GetUniqueIdQueryParameter()}",
            response.Headers.Location?.OriginalString
        );
    }

    [Fact]
    public async Task Get_ValidRequest_RendersExpectedContent()
    {
        // Arrange
        var state = CreateNewState();
        var journeyInstance = await CreateJourneyInstance(state);

        var oneLoginUser = await TestData.CreateOneLoginUser(verified: true);

        var ticket = CreateOneLoginAuthenticationTicket(
            SignInJourneyHelper.AuthenticationOnlyVtr,
            oneLoginUser
        );
        await GetSignInJourneyHelper().OnOneLoginCallback(journeyInstance, ticket);

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"/not-found?{journeyInstance.GetUniqueIdQueryParameter()}"
        );

        // Act
        var response = await HttpClient.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
