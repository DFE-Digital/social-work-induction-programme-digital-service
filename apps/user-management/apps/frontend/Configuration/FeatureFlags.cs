namespace Dfe.Sww.Ecf.Frontend.Configuration;

public class FeatureFlags
{
    public bool EnableDeveloperExceptionPage { get; set; }
    public bool EnableHttpStrictTransportSecurity { get; set; }
    public bool EnableContentSecurityPolicyWorkaround { get; set; }
    public bool EnableForwardedHeaders { get; set; }
    public bool EnableMoodleIntegration { get; set; }
}
