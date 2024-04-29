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
         public Startup(IWebHostEnvironment hostingEnvironment)
        {
           _appConfiguration = AppConfigurations.Get();
           _hostingEnvironment = hostingEnvironment;
        }

        private readonly IConfigurationRoot _appConfiguration;
        private readonly IWebHostEnvironment _hostingEnvironment;
		
		public ILifetimeScope AutofacContainer { get; private set; }        

		//autofac added
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register our custom directly with Autofac 
                    builder.RegisterAssemblyTypes(typeof(Startup).Assembly);
                    builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                        .Where(t => t.Name.EndsWith("Common") )
                        .AsImplementedInterfaces()
                        .InstancePerRequest();
           
            
        }
      
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomAPIController();
            services.AddIoC();
            services.AddCustomHttpCleint();
            services.AddCustomInMemory();
            services.AddCustomSwagger();
            services.AddCustomHealthCheck();
            services.AddCustomCors();
            services.AddCustomCompression();
            services.AddCustomHsts();
            services.AddCustomIpRateLimit();
            $$MgsBrokerStart$$
            services.AddIntegrationServices(_appConfiguration);
            services.AddEventBus(_appConfiguration);
            $$MgsBrokerEnd$$
            $$SchedulerService$$ services.AddSchedular(_appConfiguration);
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.CreateLogger<Startup>().LogDebug("Using PATH BASE '{pathBase}'");
            
            var App = new CustomExtensionStartup();
            
            App.ConfigureDefualt(app);			
        }		

    }
}
