using Xunit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace $$namespace$$;

public class PerformanceApiTest : IClassFixture<WebApplicationFactory<$$Program$$>>, IDisposable
{
    protected IConfiguration configuration;
    protected HttpClient? _client;
    protected int usersCount, maxThreads, timeLimit;

    public PerformanceApiTest()
    {
        configuration = GetTestDataConfiguration();
        timeLimit = Convert.ToInt32(configuration.GetSection("TimeLimit").Value);
        usersCount = Convert.ToInt32(configuration.GetSection("UsersCount").Value);
        maxThreads = Convert.ToInt32(configuration.GetSection("MaxThreads").Value);
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