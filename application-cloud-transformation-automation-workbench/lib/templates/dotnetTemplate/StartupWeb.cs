using Google.Cloud.PubSub.V1;
using Infrastructure.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Infrastructure.Common.EventBus;
using Infrastructure.Common.EventBus.Abstractions;
using Infrastructure.Common.StartUp;

namespace $namespace$
{
     public class Startup
    {
         public Startup()
        {
           _appConfiguration = AppConfigurations.Get();
        }

        private readonly IConfigurationRoot _appConfiguration;

        // This method gets called by the runtime. Use this method to add services to the container.
         public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddCustomHttpCleint();
            services.AddCustomInMemory();
            services.AddCustomSwagger();
            services.AddCustomHealthCheck();
            services.AddCustomCors();
            services.AddCustomCompression();
            services.AddCustomHsts();
            services.AddCustomIpRateLimit();            
            services.AddOptions()
            .AddSession()
            .AddControllers();
            $$MgsBrokerStart$$
            services.AddIntegrationServices(_appConfiguration);
            services.AddEventBus(_appConfiguration);
            $$MgsBrokerEnd$$
            $$SchedulerService$$ services.AddSchedular(_appConfiguration);            			
			var container = new ContainerBuilder();
            container.Populate(services);
            container.RegisterAssemblyTypes(typeof(Startup).Assembly);

            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.CreateLogger<Startup>().LogDebug("Using PATH BASE '{pathBase}'");
            
            var customApp = new CustomExtensionStartup();

            customApp.ConfigureBasePath(app);     
            app.UseStaticFiles();
            app.UseSession();     
            app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.RegisterRoutes();
            ConfigureMiddleware(app);
        }
		
		public void ConfigureMiddleware(IApplicationBuilder app)
        {
            app.UseApplicationStartMiddleware();
        }

    }
}
