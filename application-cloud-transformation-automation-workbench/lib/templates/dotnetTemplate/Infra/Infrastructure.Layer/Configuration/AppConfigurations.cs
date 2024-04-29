using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
$$USING$$

namespace Infrastructure.Common
{
    public static class AppConfigurations
    {
        private static readonly ConcurrentDictionary<string, IConfigurationRoot> _configurationCache;

        static AppConfigurations()
        {
            _configurationCache = new ConcurrentDictionary<string, IConfigurationRoot>();
        }
		
        /// <summary>
        /// Get values from appsetting.Json
        /// </summary>
        /// <param name="path">Base directoy path</param>
        /// <param name="environmentName">Environment Names</param>
        /// <param name="addUserSecrets">secrets key to get value from cache</param>
        /// <returns></returns>
        public static IConfigurationRoot Get(string path = null, string environmentName = null, bool addUserSecrets = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = AppContext.BaseDirectory;
            }
            if (!string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("Isfunction")))
            {
                path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..");
            }
            var cacheKey = path + "#" + environmentName + "#" + addUserSecrets;
            return _configurationCache.GetOrAdd(
                cacheKey,
                _ => BuildConfiguration(path, environmentName, addUserSecrets)
            );
        }
		
        /// <summary>
        /// Configure the appsetting.json
        //// </summary>
        /// <param name="path">Base directoy path</param>
        /// <param name="environmentName">Environment Names</param>
        /// <param name="addUserSecrets">secrets key to get value from cache</param>
        /// <returns></returns>
        private static IConfigurationRoot BuildConfiguration(string path, string environmentName = null, bool addUserSecrets = false)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            if (!string.IsNullOrWhiteSpace(environmentName))
            {
                builder = builder.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
            }

            builder = builder.AddEnvironmentVariables();

            if (addUserSecrets)
            {
                builder.AddUserSecrets(typeof(AppConfigurations).Assembly, optional: true);
            }

            var config = builder.Build();

            $$BUILD_CONFIG_CODE$$
            return builder.Build();
        }
    }
	
    public class ConfigurationManager
    {
        public AppSettings AppSettings = new AppSettings();
    }
	
    public class AppSettings
    {
        public string this[string index]
        {
            get
            {
                return AppConfigurations.Get()[index];
            }
        }
    }

}
