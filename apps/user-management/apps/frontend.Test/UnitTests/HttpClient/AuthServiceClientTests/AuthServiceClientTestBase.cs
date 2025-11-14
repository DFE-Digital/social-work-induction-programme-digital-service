using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Dfe.Sww.Ecf.Frontend.HttpClients.AuthService;
using Microsoft.AspNetCore.Http;
using Moq;
using RichardSzalay.MockHttp;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.HttpClient.AuthServiceClientTests;

public abstract class AuthServiceClientTestBase
{

    protected AuthServiceClient BuildSut(MockHttpMessageHandler mockHttpMessageHandler)
    {
        var client = mockHttpMessageHandler.ToHttpClient();
        client.BaseAddress = new Uri("http://localhost");

        var claims = new List<Claim>
        {
            new Claim("organisation_id", Guid.NewGuid().ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext
        {
            User = claimsPrincipal
        };
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
        var sut = new AuthServiceClient(client, mockHttpContextAccessor.Object);

        return sut;
    }

    protected (
        MockHttpMessageHandler MockHttpMessageHandler,
        MockedRequest MockedRequest
    ) GenerateMockClient(
        HttpStatusCode statusCode,
        HttpMethod httpMethod,
        object? response,
        string route
    )
    {
        var mockHttp = new MockHttpMessageHandler();
        var request = mockHttp
            .When(httpMethod, route)
            .Respond(statusCode, "application/json", JsonSerializer.Serialize(response));

        return (mockHttp, request);
    }

    // Helper to test deserialisation errors
    protected (
        MockHttpMessageHandler MockHttpMessageHandler,
        MockedRequest MockedRequest
        ) GenerateMockClientWithRawResponse(
            HttpStatusCode statusCode,
            HttpMethod httpMethod,
            string rawContent,
            string route
        )
    {
        var mockHttp = new MockHttpMessageHandler();
        var request = mockHttp
            .When(httpMethod, route)
            .Respond(statusCode, "application/json", rawContent);

        return (mockHttp, request);
    }
}
