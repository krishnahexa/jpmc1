using Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace $namespace$
{
    public static class Program
    {
                private   static IConfigurationRoot? _appConfiguration;
        public static async Task Main(string[] args)
        {
             _appConfiguration = AppConfigurations.Get();
            var host = CreateHostBuilder(args);
            await host.RunAsync();
        }

        public static IWebHost CreateHostBuilder(string[] args) {
            return WebHost.CreateDefaultBuilder(args)
            .CaptureStartupErrors(false)
            .ConfigureAppConfiguration(x => x.AddConfiguration(_appConfiguration))
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureLogging(logging => {              
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
            })
            .UseStartup<Startup>()
            .ConfigureKestrel(o => {
                            o.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(30);
                            o.Limits.MaxRequestBodySize = Convert.ToInt64(_appConfiguration["MaxRequestBodySize"]);
                            o.Limits.MaxRequestBufferSize = Convert.ToInt64(_appConfiguration["MaxRequestBufferSize"]);

            })
           // .UseIISIntegration()
            .Build();
        }
    }
    
}
    




