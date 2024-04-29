using Autofac;
using Autofac.Extensions.DependencyInjection;
using Infrastructure.Common;
using Infrastructure.Common.StartUp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Net.Http;
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
    services.AddControllersWithViews();
    services.AddIoC()
            .AddCustomHttpCleint()
            .AddCustomInMemory()
            .AddCustomSwagger()
            .AddCustomHealthCheck()
            .AddCustomCors()
            .AddCustomCompression()
            .AddCustomHsts()
            .AddCustomIpRateLimit()             
            .AddHttpContextAccessor()          
            .AddOptions()
            .AddSession()
             $$CONFIG$$
            .AddControllers();
             
			
			 
}


// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
void Configure(IApplicationBuilder app)
{
   
            var customApp = new CustomExtensionStartup();
            customApp.ConfigureBasePath(app); 

            app.UseStaticFiles()
               .UseSession()     
               .UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax })
               .UseRouting()
               .UseAuthentication()
               .UseAuthorization()
               .RegisterRoutes();

            ConfigureMiddleware(app);
}

void ConfigureMiddleware(IApplicationBuilder app)
{
    app.UseApplicationStartMiddleware();
}

namespace $$namespace$$
{
    public partial class Program
    {
        public static string Namespace = typeof(Program).Namespace;
        public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);
    }
}


