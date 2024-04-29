namespace Infrastructure.Common
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Microsoft.AspNetCore.Http;
    using System.Net;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Hosting;
    using Infrastructure.Common.Exceptions;
    using KeyNotFoundException = KeyNotFoundException;
    using System.Security.Cryptography.Xml;
    using System.Diagnostics;

    public class HttpGlobalExceptionFilter : IAsyncExceptionFilter
    {
        private readonly IWebHostEnvironment env;
        private readonly ILogger<HttpGlobalExceptionFilter> logger;

        public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            this.env = env;
            this.logger = logger;
        }

        Task IAsyncExceptionFilter.OnExceptionAsync(ExceptionContext context)
        {
            logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);
            
            var statusCode = context.Exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                NotImplementedException => StatusCodes.Status501NotImplemented,
                ValidationException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,

                _ => StatusCodes.Status500InternalServerError
            };

            
            if (this.env.EnvironmentName.ToLower().Contains("dev") || this.env.EnvironmentName.ToLower().Contains("qa") || this.env.EnvironmentName.ToLower().Contains("sit"))
            {
                context.Result = new ObjectResult(new
                {
                    error = context.Exception.Message,
                    stackTrace = context.Exception.StackTrace
                })
                {
                    StatusCode = statusCode
                };
            }
            else
            {
                context.Result = new ObjectResult(new
                {
                    error = context.Exception.Message,
                })
                {
                    StatusCode = statusCode
                };
            }

            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }

      

        private class JsonErrorResponse
        {
            public string[] Messages { get; set; }

            public object DeveloperMessage { get; set; }
        }
    }
}