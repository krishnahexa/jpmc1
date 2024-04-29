using Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using $APP_NAME$.Controller;
$$USING$$

namespace $APP_NAME$.Helper
{
    public static class AppIOCExtentionMethod
    {
        public static IServiceCollection AddIoC(this IServiceCollection services)
        {
            services.AddScoped<ConfigurationManager>();

            #region IOC

            $SERVICE_IMPL$            

            #endregion

            return services;
        }
    }
}
