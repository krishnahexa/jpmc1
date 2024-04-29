using AspNetCoreRateLimit;
using Infrastructure.Common.Filters;
using Infrastructure.Common.StartUp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System.IO.Compression;
$$ImportStatement$$

namespace Infrastructure.Common
{
    public static class StartUpExtensionMethods
    {
        /// <summary>
        /// Add custom Host
        /// </summary>
        /// <param name="services">IServiceCollection object</param>
        /// <returns></returns>
        public static IServiceCollection AddCustomHsts(this IServiceCollection services)
        {
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });
            return services;
        }
        /// <summary>
        /// Enable Health check
        /// </summary>
        /// <param name="services">IServiceCollection object</param>
        /// <returns></returns>
        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services)
        {
            var hcBuilder = services.AddHealthChecks();
            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            return services;
        }
        /// <summary>
        /// Enable Httpclient
        /// </summary>
        /// <param name="services">IServiceCollection object</param>
        /// <returns></returns>
        public static IServiceCollection AddCustomHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddHttpContextAccessor();

            return services;
        }
        /// <summary>
        /// Add InMemory and Distributed Cache
        /// </summary>
        /// <param name="services">IServiceCollection object</param>
        /// <returns></returns>
        public static IServiceCollection AddCustomInMemory(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddMemoryCache();

            return services;
        }
        /// <summary>
        /// Add Custom Swagger (Enable/Disable)
        /// </summary>
        /// <param name="services">IServiceCollection object</param>
        /// <returns></returns>
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            var _appConfiguration = AppConfigurations.Get();
            if (Convert.ToBoolean(_appConfiguration["EnableSwagger"]))
            {
                var pathBase = AppConfigurations.Get()["PATH_BASE"];
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = pathBase, Version = "v1" });
                });

            }
            return services;
        }

        /// <summary>
        /// Enable Request log and Response Log
        /// </summary>
        /// <param name="services">IServiceCollection object</param>
        /// <returns></returns>
        public static IServiceCollection AddCustomAPIController(this IServiceCollection services)
        {
            var _appConfiguration = AppConfigurations.Get();
            services.AddControllers(config =>
            {
                if (Convert.ToBoolean(_appConfiguration["EnableRequestLogFilter"]))
                {
                    config.Filters.Add(typeof(RequestLogFilter));
                }
                if (Convert.ToBoolean(_appConfiguration["EnableResponseLogFilter"]))
                {
                    config.Filters.Add(typeof(ResponseLogFilter));
                }
                config.Filters.Add(typeof(HttpGlobalExceptionFilter));
                config.EnableEndpointRouting = false;
                config.OutputFormatters.RemoveType<Microsoft.AspNetCore.Mvc.Formatters.HttpNoContentOutputFormatter>();
                AuthorizationFilterExtention.AddCustomAutherization(config);
            }).AddNewtonsoftJson(
            options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            }
        ).ConfigureApiBehaviorOptions(o => { o.SuppressModelStateInvalidFilter = true; }); 

            // .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true)
            //.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            return services;
        }
        /// <summary>
        /// Add Cors 
        /// </summary>
        /// <param name="services">IServiceCollection object</param>
        /// <returns></returns>
        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            var _defaultCorsPolicyName = AppConfigurations.Get()["CorsPolicyName"];
            var _appConfiguration = AppConfigurations.Get();
            if (_appConfiguration["AllowedHosts"] == "*")
            {
                services.AddCors(
                 options => options.AddPolicy(
                     _defaultCorsPolicyName,
                     builder => builder
                         .AllowAnyHeader()
                         .AllowAnyMethod()
                         .SetIsOriginAllowedToAllowWildcardSubdomains()
                         .AllowCredentials()
                 )
             );
            }
            else
            {
                services.AddCors(
                   options => options.AddPolicy(
                       _defaultCorsPolicyName,
                       builder => builder
                           .WithOrigins(
                               // AllowedHosts :CorsOrigins in appsettings.json can contain more than one address separated by comma.
                               _appConfiguration["AllowedHosts"]
                                   .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                   .ToArray()
                           )
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .SetIsOriginAllowedToAllowWildcardSubdomains()
                           .AllowCredentials()
                   )
               );
            }
            return services;
        }
        /// <summary>
        /// Response Compression
        /// </summary>
        /// <param name="services">IServiceCollection object</param>
        /// <returns></returns>
        public static IServiceCollection AddCustomCompression(this IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });


            return services;
        }
        /// <summary>
        /// Custom IP limit to access the endpoints
        /// </summary>
        /// <param name="services">IServiceCollection object</param>
        /// <returns></returns>
        public static IServiceCollection AddCustomIpRateLimit(this IServiceCollection services)
        {
            var _appConfiguration = AppConfigurations.Get();
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(_appConfiguration.GetSection("IpRateLimiting"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            services.AddInMemoryRateLimiting();

            return services;
        }
		
		    $$EventCode$$
          $$JobSchedular$$         		 
	//CacheStart
	    public static IServiceCollection AddCache(this IServiceCollection services,
         IConfiguration configuration, IWebHostEnvironment _hostingEnvironment)
         {
            if (_hostingEnvironment.EnvironmentName == "")
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                //RedisStart
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configuration.GetSection("RedisCache")["Connection"];
                    options.InstanceName = configuration.GetSection("RedisCache")["InstanceName"];
                });
                //RedisEnd
                //MangoDBStart
                services.AddMongoDbCache(options =>
                {
                    options.ConnectionString = configuration.GetSection("MongoDbCache")["Connection"];
                    options.DatabaseName = configuration.GetSection("MongoDbCache")["DatabaseName"];
                    options.CollectionName = configuration.GetSection("MongoDbCache")["CollectionName"];
                    options.ExpiredScanInterval = TimeSpan.FromMinutes(10);
                });
                //MangoDBEnd
				//HazelcastStart
                 services.AddSingleton<IDistributedCache, HazelCastCache>(sp =>
                 {
                    var options = new HazelcastOptionsBuilder()
                    .With(option =>
                    {
                    option.ClusterName = configuration.GetSection("HazelcastCache")["Cluster"];
                    option.Networking.Addresses.Add(configuration.GetSection("HazelcastCache")["Connection"]);
                    }).Build();
                    var mapName = configuration.GetSection("HazelcastCache")["Map"];
                    return new HazelCastCache(options, mapName);
                });
                //HazelcastEnd
            }
            services.AddSingleton<ICache, DistributedCache>();
            return services;
        }
        //CacheEnd
    }


    public class CustomExtensionStartup
    {
        /// <summary>
        /// Base path configuration
        /// </summary>
        /// <param name="app">application's service container</param>
        public virtual void ConfigureBasePath(IApplicationBuilder app)
        {
            var pathBase = AppConfigurations.Get()["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase($"/{pathBase}");

                //app.Use((context, next) =>
                //{
                //    context.Request.PathBase = $"/{pathBase}";
                //    return next();
                //});
            }
        }
        /// <summary>
        /// Enable Authentication adn Authorization
        /// </summary>
        /// <param name="app">application's service container</param>
        public virtual void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
        /// <summary>
        /// Configure Routing
        /// </summary>
        /// <param name="app">application's service container</param>
        public virtual void ConfigureRoute(IApplicationBuilder app)
        {
            app.UseRouting();
        }
        /// <summary>
        /// Configure Cors policy
        /// </summary>
        /// <param name="app">application's service container</param>
        public virtual void ConfigureCors(IApplicationBuilder app)
        {
            var _defaultCorsPolicyName = AppConfigurations.Get()["CorsPolicyName"];
            app.UseCors(_defaultCorsPolicyName);
        }
        /// <summary>
        /// Configure Https and redirection
        /// </summary>
        /// <param name="app">application's service container</param>
        public virtual void ConfigureHttps(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseHsts();
        }
        /// <summary>
        /// Enable Health check
        /// </summary>
        /// <param name="app">application's service container</param>
        public virtual void ConfigureAddHealthCheck(IApplicationBuilder app)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true
            })
              .UseHealthChecks("/", new HealthCheckOptions
              {
                  Predicate = _ => true,
                  //ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
              });
        }
        /// <summary>
        /// Configure Swagger
        /// </summary>
        /// <param name="app">application's service container</param>
        public virtual void ConfigureSwagger(IApplicationBuilder app)
        {
            var _appConfiguration = AppConfigurations.Get();
            if (Convert.ToBoolean(_appConfiguration["EnableSwagger"]))
            {
                var pathBase = AppConfigurations.Get()["PATH_BASE"];
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{pathBase} v1");
                    options.DisplayRequestDuration();
                    // options.RoutePrefix = pathBase;
                });
            }
        }
        /// <summary>
        /// Configure Endpoints
        /// </summary>
        /// <param name="app">application's service container</param>
        public virtual void ConfigureEndPoint(IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        /// <summary>
        /// Enable IpRateLimiting
        /// </summary>
        /// <param name="app">application's service container</param>
        public virtual void ConfigureIpRateLimiting(IApplicationBuilder app)
        {
            app.UseIpRateLimiting();
        }
        /// <summary>
        /// Enable/Configure Compresssion
        /// </summary>
        /// <param name="app">application's service container</param>
        public virtual void ConfigureCompression(IApplicationBuilder app)
        {
            app.UseResponseCompression();
        }
        /// <summary>
        /// Configure Security in Header Request
        /// </summary>
        /// <param name="app">application's service container</param>
        public virtual void ConfigureSecurityHeaders(IApplicationBuilder app)
        {
            app.UseSecurityHeadersMiddleware(new SecurityHeadersBuilder()
           .AddDefaultSecurePolicy()
           .AddCustomHeader("X-My-Custom-Header", "So amaze")
         );
        }
        /// <summary>
        /// Helper method to enable all default service
        /// </summary>
        /// <param name="app">application's service container</param>
        public virtual void ConfigureDefualt(IApplicationBuilder app)
        {
            ConfigureBasePath(app);
            ConfigureSwagger(app);
            ConfigureCors(app);
            ConfigureAddHealthCheck(app);
            ConfigureCompression(app);
            ConfigureSecurityHeaders(app);
            ConfigureHttps(app);
            ConfigureIpRateLimiting(app);
            ConfigureRoute(app);
            ConfigureAuth(app);
            ConfigureEndPoint(app);
            ServiceHelper.Services = app.ApplicationServices;
        }
    }



}
