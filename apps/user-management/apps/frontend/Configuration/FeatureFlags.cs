namespace Dfe.Sww.Ecf.Frontend.Configuration;

public class FeatureFlags
{
    public bool EnableDeveloperExceptionPage { get; init; }
    public bool EnableHttpStrictTransportSecurity { get; init; }
    public bool EnableContentSecurityPolicyWorkaround { get; init; }
    public bool EnableForwardedHeaders { get; init; }
    public bool EnableMoodleIntegration { get; init; }
    public bool EnablePlusEmailStripping { get; init; }
}
