using System.Text.Json;
using System.Text.Json.Serialization;
using Dfe.Sww.Ecf.UiCommon.FormFlow;
using Microsoft.AspNetCore.Authentication;

namespace Dfe.Sww.Ecf.AuthorizeAccess;

[method: JsonConstructor]
public class SignInJourneyState(
    string redirectUri,
    string serviceName,
    string serviceUrl,
    string? linkingToken = null
)
{
    public const string JourneyName = "SignInJourney";

    public static JourneyDescriptor JourneyDescriptor { get; } =
        new(JourneyName, typeof(SignInJourneyState), requestDataKeys: [], appendUniqueKey: true);

    public string RedirectUri { get; } = redirectUri;

    public string ServiceName { get; } = serviceName;

    public string ServiceUrl { get; } = serviceUrl;

    public string? LinkingToken { get; } = linkingToken;

    [JsonConverter(typeof(AuthenticationTicketJsonConverter))]
    public AuthenticationTicket? AuthenticationTicket { get; set; }

    [JsonConverter(typeof(AuthenticationTicketJsonConverter))]
    public AuthenticationTicket? OneLoginAuthenticationTicket { get; set; }

    public bool AttemptedIdentityVerification { get; set; }

    [JsonInclude]
    public bool IdentityVerified { get; private set; }

    [JsonInclude]
    public string[][]? VerifiedNames { get; private set; }

    [JsonInclude]
    public DateOnly[]? VerifiedDatesOfBirth { get; private set; }

    [JsonInclude]
    public bool? HaveNationalInsuranceNumber { get; private set; }

    [JsonInclude]
    public string? NationalInsuranceNumber { get; private set; }

    public void Reset()
    {
        AuthenticationTicket = null;
        OneLoginAuthenticationTicket = null;
        VerifiedNames = null;
        VerifiedDatesOfBirth = null;
    }

    public void SetVerified(string[][] verifiedNames, DateOnly[] verifiedDatesOfBirth)
    {
        IdentityVerified = true;
        VerifiedNames = verifiedNames;
        VerifiedDatesOfBirth = verifiedDatesOfBirth;
    }

    // Used by the Debug page
    public void ClearVerified()
    {
        IdentityVerified = false;
        VerifiedNames = null;
        VerifiedDatesOfBirth = null;
    }

    public void SetNationalInsuranceNumber(
        bool haveNationalInsuranceNumber,
        string? nationalInsuranceNumber
    )
    {
        if (haveNationalInsuranceNumber && nationalInsuranceNumber is null)
        {
            throw new ArgumentException(
                "National Insurance number must be specified.",
                nameof(nationalInsuranceNumber)
            );
        }

        HaveNationalInsuranceNumber = haveNationalInsuranceNumber;
        NationalInsuranceNumber = haveNationalInsuranceNumber ? nationalInsuranceNumber! : null;
    }
}

public class AuthenticationTicketJsonConverter : JsonConverter<AuthenticationTicket>
{
    private readonly TicketSerializer _ticketSerializer = TicketSerializer.Default;

    public override AuthenticationTicket? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var bytes = reader.GetBytesFromBase64();
            return _ticketSerializer.Deserialize(bytes);
        }

        throw new JsonException($"Unknown TokenType: '{reader.TokenType}'.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        AuthenticationTicket value,
        JsonSerializerOptions options
    )
    {
        var bytes = _ticketSerializer.Serialize(value);
        writer.WriteBase64StringValue(bytes);
    }
}
