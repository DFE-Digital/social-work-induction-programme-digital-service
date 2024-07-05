using Microsoft.Extensions.Configuration;

namespace DfeSwwEcf.NotificationService.Tests.FunctionalTests.Configuration
{
    public class ConfigAccessor
    {
        private static IConfigurationRoot? _root;

        private static IConfigurationRoot GetIConfigurationRoot()
        {
            if (_root == null)
            {
                _root = new ConfigurationBuilder()
                    .AddJsonFile("FunctionalTests/appsettings.json", optional: false, reloadOnChange:true)
                    .AddEnvironmentVariables()
                    .Build();
            }

            return _root;
        }

        public static ConfigModel GetApplicationConfiguration()
        {
            var configuration = new ConfigModel();

            var iConfig = GetIConfigurationRoot();

            iConfig.Bind(configuration);

            return configuration;
        }
    }
}
