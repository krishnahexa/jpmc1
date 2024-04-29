using Autofac.Extensions.DependencyInjection;
using Infrastructure.Common;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
$$USING$$



var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Directory.GetCurrentDirectory(),
});

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

var _appConfiguration = AppConfigurations.Get();
builder.Configuration.AddConfiguration(_appConfiguration);

builder.Logging.ClearProviders()
                .AddConsole()
                .AddDebug();

$$INSERTCODE$$

// Add services to the container.
ConfigureServices(builder.Services);


var app = builder.Build();

// Configure the HTTP request pipeline.
Configure(app);

app.Run();


void ConfigureServices(IServiceCollection services)
{
    services.AddCustomAPIController();
    services.AddCustomHttpCleint();
    services.AddCustomInMemory();
    services.AddCustomSwagger();
    services.AddCustomHealthCheck();
    services.AddCustomCors();
    services.AddCustomCompression();
    services.AddCustomHsts();
    services.AddCustomIpRateLimit();    
    $$CONFIG$$

}


// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
void Configure(IApplicationBuilder app)
{
   var App = new CustomExtensionStartup();
   App.ConfigureDefualt(app);
}

namespace $$namespace$$
{
    public partial class Program
    {
        public static string Namespace = typeof(Program).Namespace;
        public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
    }
}

