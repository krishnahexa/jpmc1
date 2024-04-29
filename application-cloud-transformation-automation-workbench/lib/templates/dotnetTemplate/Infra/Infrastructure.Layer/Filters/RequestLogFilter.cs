
namespace Infrastructure.Common
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    public class RequestLogFilter : IAsyncActionFilter
    {
        private readonly ILogger<RequestLogFilter> logger;

        public RequestLogFilter (ILogger<RequestLogFilter> logger)
        {
            this.logger = logger;
        }
        public async Task OnActionExecutionAsync( ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context == null)
            {
                throw new ArgumentException("Filter Context is null");
            }
            var user = context.HttpContext.User.Identity?.Name;
            var iPAddress = context.HttpContext.Connection.RemoteIpAddress.ToString() ?? "127.0.0.1";
            var request = context.ActionArguments.Values;
            var requestJosn = JsonSerializer.Serialize(request); 

            logger.LogInformation($" User - {user} IPAddress - {iPAddress} Request - { requestJosn}");

            if (next != null) {
                await next().ConfigureAwait(false);
            }
        }
    }
}
