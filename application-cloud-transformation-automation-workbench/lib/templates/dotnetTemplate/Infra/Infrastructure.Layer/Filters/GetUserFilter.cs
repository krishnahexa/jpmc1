
namespace Infrastructure.Common
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;
    public class GetUserFilter : IAsyncActionFilter
    {
        private readonly ILogger<GetUserFilter> logger;

        public GetUserFilter(ILogger<GetUserFilter> logger)
        {
            this.logger = logger;
        }
        public async  Task OnActionExecutionAsync( ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context == null)
            {
                throw new ArgumentException("Filter Context is null");
            }
            var user = context.HttpContext.User.Identity?.Name;
           
            //DO TO User Session filter

            logger.LogInformation($" User - {user}  filter");

            if (next != null) {
                await next().ConfigureAwait(false);
            }
        }
    }
}
