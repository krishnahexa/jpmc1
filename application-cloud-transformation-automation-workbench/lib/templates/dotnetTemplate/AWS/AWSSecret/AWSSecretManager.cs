using System.IO;
using System.Text.Json;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

public class AmazonSecretsManagerConfigurationProvider : ConfigurationProvider
{
    private readonly string _region;
    private readonly string _secretName;
    
    private readonly MyApiCredentials _myApiCredentials;

   
    public AmazonSecretsManagerConfigurationProvider(string region, string secretName)
    {
        _region = region;
        _secretName = secretName;
    }

    public override void Load()
    {
        var secret = GetSecret();

        Data = JsonSerializer.Deserialize<Dictionary<string, string>>(secret);
    }

   private string GetSecret()
    {
        var request = new GetSecretValueRequest
        {
            SecretId = _secretName,
            VersionStage = "AWSCURRENT" // VersionStage defaults to AWSCURRENT if unspecified.
        };

        using (var client = 
		new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(_region)))
        {
            var response = client.GetSecretValueAsync(request).Result;

            string secretString;
            if (response.SecretString != null)
            {
                secretString = response.SecretString;
            }
            else
            {
                var memoryStream = response.SecretBinary;
                var reader = new StreamReader(memoryStream);
                secretString = 
		System.Text.Encoding.UTF8
			.GetString(Convert.FromBase64String(reader.ReadToEnd()));
            }

            return secretString;

            
        }
    }
}

   

public class AmazonSecretsManagerConfigurationSource : IConfigurationSource
{
    private readonly string _region;
    private readonly string _secretName;

    public AmazonSecretsManagerConfigurationSource(string region, string secretName)
    {
        _region = region;
        _secretName = secretName;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new AmazonSecretsManagerConfigurationProvider(_region, _secretName);
    }
}

public class MyApiCredentials
{
    public string ApiKey { get; set; }
    public string UserId { get; set; }
    public string Password { get;set; }
}


