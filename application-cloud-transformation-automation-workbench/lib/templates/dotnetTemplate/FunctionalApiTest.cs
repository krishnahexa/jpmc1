using Xunit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace $$namespace$$;

public class FunctionalApiTest : IClassFixture<WebApplicationFactory<$$Program$$>> , IDisposable
{
    protected HttpClient? _client;
    protected IConfiguration configuration;
    public FunctionalApiTest()
    {
        configuration = GetTestDataConfiguration();
    }
    public IConfiguration GetTestDataConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(@"appsettings.json", true, false)
                .AddEnvironmentVariables()
                .Build();
        }
    public void Dispose()
     {
        _client = null;
        configuration = null;
     }


}