namespace Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Configuration;

public class FeatureFlagOptions
{
    public const string ConfigurationKey = "FeatureFlags";

    public bool SupportEndToEndTesting { get; init; }
    public bool RequiresDbConnection { get; init; }
    public bool EnableDeveloperExceptionPage { get; init; }
    public bool EnableMigrationsEndpoint { get; init; }
    public bool EnableErrorExceptionHandler { get; init; }
    public bool EnableContentSecurityPolicyWorkaround { get; init; }
    public bool EnableDfeAnalytics { get; init; }
    public bool EnableSwagger { get; init; }
    public bool EnableSentry { get; init; }
    public bool EnableHttpStrictTransportSecurity { get; init; }
    public bool EnableForwardedHeaders { get; init; }
    public bool EnableMsDotNetDataProtectionServices { get; init; }
    public bool EnableOpenIdCertificates { get; init; }
    public bool EnableOneLoginCertificateRotation { get; init; }
    public bool EnableDevelopmentOpenIdCertificates { get; init; }
}
