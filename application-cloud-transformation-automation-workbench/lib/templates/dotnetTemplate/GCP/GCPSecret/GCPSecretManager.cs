using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Microsoft.Extensions.Configuration;

namespace GoogleSecretManagerConfigurationProvider
{
    public class SecretManagerConfigurationProvider : ConfigurationProvider
    {
        private readonly SecretManagerServiceClient _client;
        private readonly string _projectId;

        public SecretManagerConfigurationProvider()
        {
            _client = SecretManagerServiceClient.Create();
            _projectId = Google.Api.Gax.Platform.Instance().ProjectId;
        }

        public SecretManagerConfigurationProvider(SecretManagerServiceClient client, string projectId)
        {
            _client = client;
            _projectId = projectId;
        }

        /// <summary>
        /// Load Secrets from Google Secret Manager
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "N/A")]
        public override void Load()
        {
            if(string.IsNullOrEmpty(_projectId))
            {
                return; // skip for local debug
            }

            var secrets = _client.ListSecrets(new ProjectName(_projectId));
            foreach (var secret in secrets)
            {
                try
                {
                    var secretVersionName = new SecretVersionName(secret.SecretName.ProjectId, secret.SecretName.SecretId, "latest");
                    var secretVersion = _client.AccessSecretVersion(secretVersionName);
                    Set(NormalizeDelimiter(secret.SecretName.SecretId), secretVersion.Payload.Data.ToStringUtf8());
                }
                catch (Grpc.Core.RpcException)
                {
                    // Ignore. This might happen if secret is created but it has no versions available
                }
            }
        }

        private static string NormalizeDelimiter(string key)
        {
            return key.Replace("__", ConfigurationPath.KeyDelimiter);
        }
    }
    public class SecretManagerConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// Creates new SecretsManagerConfigurationProvider
        /// </summary>
        /// <param name="builder"></param>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new SecretManagerConfigurationProvider();
        }
    }
    public static class SecretManagerConfigurationExtensions
    {
        /// <summary>
        /// Add SecretManagerConfigurationSource to the build configuration
        /// </summary>
        /// <param name="configurationBuilder"></param>
        public static IConfigurationBuilder AddGoogleSecretsManager(this IConfigurationBuilder configurationBuilder, string v, string v1)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }

            configurationBuilder.Add(new SecretManagerConfigurationSource());

            return configurationBuilder;
        }
    }
    
}
