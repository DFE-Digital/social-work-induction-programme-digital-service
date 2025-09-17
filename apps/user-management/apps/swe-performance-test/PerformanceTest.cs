using Microsoft.Extensions.Configuration;
using Abstracta.JmeterDsl.Core.Assertions;
using static Abstracta.JmeterDsl.JmeterDsl;

namespace Dfe.Sww.Ecf.Swe.Performance.Test;
public class PerformanceTest
{
    private TextWriter? _originalConsoleOut;
    private string _bearerToken;
    private string? _baseUrl;

    [SetUp]
    public async Task SetUp()
    {
        _originalConsoleOut = Console.Out ?? throw new InvalidOperationException("Console.Out is null");
        Console.SetOut(TestContext.Progress);

        var builder = new ConfigurationBuilder()
            .AddUserSecrets<PerformanceTest>()
            .AddEnvironmentVariables();

        IConfiguration configuration = builder.Build();

        string? environment = configuration["ENVIRONMENT"]?.ToLower();

        if (string.IsNullOrEmpty(environment))
        {
            throw new ArgumentNullException(nameof(environment), "Environment variable ENVIRONMENT is not set.");
        }

        if (environment == "preprod")
        {
            _baseUrl = "https://api-external-preprod.socialworkengland.org.uk";
        }
        else if (environment == "production")
        {
            _baseUrl = "https://api-external.socialworkengland.org.uk";
        }
        else
        {
            throw new ArgumentException("Invalid environment specified. Use 'preprod' or 'production'.");
        }

        string? clientId = configuration["ClientId"];
        string? clientSecret = configuration["ClientSecret"];
        string? accessTokenUrl = configuration["AccessTokenURL"];

        if (string.IsNullOrEmpty(clientId))
        {
            throw new ArgumentNullException(nameof(clientId), "Client ID cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(clientSecret))
        {
            throw new ArgumentNullException(nameof(clientSecret), "Client Secret cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(accessTokenUrl))
        {
            throw new ArgumentNullException(nameof(accessTokenUrl), "Access Token URL cannot be null or empty.");
        }

        _bearerToken = await GetTokenAsync(accessTokenUrl, clientId, clientSecret);
    }

    [TearDown]
    public void TearDown()
    {
        Console.SetOut(_originalConsoleOut!);
    }

    [Test]
    public void LoadTest()
    {
        string results = Directory.GetCurrentDirectory() + "/performance-test-results";

        var stats = TestPlan(
                ThreadGroup(1, TimeSpan.FromSeconds(30),
                    HttpSampler($"{_baseUrl}/GetSocialWorkerById")
                        .Param("swid", "${__Random(1,100000)}")
                        .Header("Authorization", $"Bearer {_bearerToken}")
                        .Children(
                            ResponseAssertion()
                                .FieldToTest(DslResponseAssertion.TargetField.ResponseMessage)
                                .ContainsSubstrings("OK")
                        )
                ),
                JtlWriter(results, DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss").Replace(":", "-") + "-performance-report")

                // *useful for debugging*
                //     .WithAllFields(),
                // ResultsTreeVisualizer()
            ).Run();

        Assert.That(stats.Overall.SampleTimePercentile99, Is.LessThan(TimeSpan.FromSeconds(5)));
    }

    private async Task<string> GetTokenAsync(string tokenEndpoint, string clientId, string clientSecret)
    {
        using (var client = new HttpClient())
        {
            var parameters = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "grant_type", "client_credentials" }
            };

            using var encodedContent = new FormUrlEncodedContent(parameters);
            var response = await client.PostAsync(tokenEndpoint, encodedContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve token");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenObj = System.Text.Json.JsonDocument.Parse(responseContent);

            if (tokenObj.RootElement.TryGetProperty("access_token", out var tokenElement))
            {
                return tokenElement.GetString() ?? throw new Exception("Token was null");
            }

            throw new Exception("Access token not found in response.");
        }
    }
}
