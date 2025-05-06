namespace Dfe.Sww.Ecf.AuthorizeAccess.Infrastructure.Configuration;

public class FeatureFlags
{
    public bool SupportEndToEndTesting { get; set; }
    public bool RequiresDbConnection { get; set; }
    public bool EnableDeveloperExceptionPage { get; set; }
    public bool EnableMigrationsEndpoint { get; set; }
    public bool EnableErrorExceptionHandler { get; set; }
    public bool EnableContentSecurityPolicyWorkaround { get; set; }
    public bool EnableDfeAnalytics { get; set; }
    public bool EnableSwagger { get; set; }
    public bool EnableSentry { get; set; }
    public bool EnableHttpStrictTransportSecurity { get; set; }
    public bool EnableForwardedHeaders { get; set; }
    public bool EnableMsDotNetDataProtectionServices { get; set; }
    public bool EnableOpenIdCertificates { get; set; }
}
