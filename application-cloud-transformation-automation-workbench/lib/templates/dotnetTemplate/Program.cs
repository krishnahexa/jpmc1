using Infrastructure.Common;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Autofac.Extensions.DependencyInjection;

namespace $namespace$
{
    public static class Program
    {
      private   static IConfigurationRoot _appConfiguration;
        public static async Task Main(string[] args)
        {
             _appConfiguration = AppConfigurations.Get();
            var host = CreateHostBuilder(args, _appConfiguration).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args,IConfigurationRoot _appConfiguration) =>
            Host.CreateDefaultBuilder(args)
			 .UseContentRoot(Directory.GetCurrentDirectory())
			.UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureLogging(logging => {              
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
            })
                .ConfigureWebHostDefaults(webBuilder => {
                    if (Convert.ToBoolean(_appConfiguration["UseKestrel"])) {
                        webBuilder.ConfigureKestrel(o => {
                            o.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(30);
                            o.Limits.MaxRequestBodySize = Convert.ToInt64(_appConfiguration["MaxRequestBodySize"]);
                            o.Limits.MaxRequestBufferSize = Convert.ToInt64(_appConfiguration["MaxRequestBufferSize"]);

                        });
                        webBuilder.UseStartup<Startup>();
                    }
                    else {                        
                        webBuilder.UseStartup<Startup>()
                        .UseIISIntegration();
                    }
                });
    }
}



