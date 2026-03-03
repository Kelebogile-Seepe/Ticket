using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BeyonceConcert.Data
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                sw.Stop();

                var traceId = httpContext.TraceIdentifier;
                var userId = httpContext.User?.Identity?.IsAuthenticated == true
                    ? httpContext.User.FindFirst("sub")?.Value ?? httpContext.User.Identity?.Name
                    : null;

                _logger.LogError(ex,
                    "Unhandled exception. TraceId={TraceId} UserId={UserId} Path={Path} ElapsedMs={ElapsedMs}",
                    traceId,
                    userId,
                    httpContext.Request.Path,
                    sw.ElapsedMilliseconds);

                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsync("An internal error occurred.");
            }
        }
    }

}