using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Common.StartUp
{

    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly SecurityHeadersPolicy _policy;

        public SecurityHeadersMiddleware(RequestDelegate next, SecurityHeadersPolicy policy)
        {
            _next = next;
            _policy = policy;
        }

        public async Task Invoke(HttpContext context)
        {
            IHeaderDictionary headers = context.Response.Headers;

            foreach (var headerValuePair in _policy.SetHeaders)
            {
                headers[headerValuePair.Key] = headerValuePair.Value;
            }

            foreach (var header in _policy.RemoveHeaders)
            {
                headers.Remove(header);
            }

            await _next(context).ConfigureAwait(false);
        }
    }

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeadersMiddleware(this IApplicationBuilder app, SecurityHeadersBuilder builder)
        {
            var policy = builder.Build();
            return app.UseMiddleware<SecurityHeadersMiddleware>(policy);
        }
    }



    public class SecurityHeadersPolicy
    {
        public IDictionary<string, string> SetHeaders { get; }
             = new Dictionary<string, string>();

        public ISet<string> RemoveHeaders { get; }
            = new HashSet<string>();
    }

    public class SecurityHeadersBuilder
    {
        private readonly SecurityHeadersPolicy _policy = new SecurityHeadersPolicy();

        public SecurityHeadersBuilder AddDefaultSecurePolicy()
        {
            AddCacheHeader();
            AddFrameOptionsSameOrigin();
            AddAccessControlExposeHeadersn();
            AddContentTypeOptionsNoSniff();
            AddStrictTransportSecurityMaxAge();
            AddXXSSProtectionBlock();
            RemoveServerHeader();

            return this;
        }

        public SecurityHeadersBuilder AddCacheHeader()
        {
            _policy.SetHeaders["Cache-Control"] = "no-store, max-age=0";
            _policy.SetHeaders["Pragma"] = "no-cache";
            _policy.SetHeaders["Expires"] = "0";
            return this;
        }

        public SecurityHeadersBuilder AddContentSecurityPolicy()
        {
            var ContentSecurityPolicy = AppConfigurations.Get()["ContentSecurityPolicy"];
            _policy.SetHeaders["Content-Security-Policy"] = ContentSecurityPolicy;
            return this;
        }


        public SecurityHeadersBuilder AddFrameOptionsSameOrigin()
        {
            _policy.SetHeaders["X-Frame-Options"] = "SAMEORIGIN";
            return this;
        }

        public SecurityHeadersBuilder AddContentTypeOptionsNoSniff()
        {
            _policy.SetHeaders["X-Content-Type-Options"] = "nosniff";
            return this;
        }
        public SecurityHeadersBuilder AddStrictTransportSecurityMaxAge()
        {
            _policy.SetHeaders["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
            return this;
        }

        public SecurityHeadersBuilder AddXXSSProtectionBlock()
        {
            _policy.SetHeaders["X-XSS-Protection"] = "; mode=block";
            return this;
        }

        public SecurityHeadersBuilder AddAccessControlExposeHeadersn()
        {
            _policy.SetHeaders["Access-Control-Expose-Headers"] = "www-authenticate";
            return this;
        }

        public SecurityHeadersBuilder RemoveServerHeader()
        {
            _policy.RemoveHeaders.Add("Server");
            _policy.RemoveHeaders.Add("X-Powered-By");
            return this;
        }

        public SecurityHeadersBuilder AddCustomHeader(string header, string value)
        {
            _policy.SetHeaders[header] = value;
            return this;
        }

        public SecurityHeadersBuilder RemoveHeader(string header)
        {
            _policy.RemoveHeaders.Add(header);
            return this;
        }

        public SecurityHeadersPolicy Build()
        {
            return _policy;
        }
    }
}
