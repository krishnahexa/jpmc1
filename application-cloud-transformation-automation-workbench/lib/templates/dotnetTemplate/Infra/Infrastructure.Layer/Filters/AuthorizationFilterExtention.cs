using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace Infrastructure.Common.Filters
{
    public static class AuthorizationFilterExtention
    {
        public static Task AddCustomAuthorization(this MvcOptions options)
        {
            var _appConfiguration = AppConfigurations.Get();
            if (Convert.ToBoolean(_appConfiguration["EnableAuthorization"])) {
                options.Filters.Add(new AuthorizeFilter());
            }
            return Task.CompletedTask;
        }
    }
}
