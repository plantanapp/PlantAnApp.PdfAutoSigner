using System.Net;
using System.Text.Json;

namespace PdfAutoSigner.LocalApi.Middlewares
{
    /// <summary>
    /// Based on https://jasonwatmore.com/post/2022/01/17/net-6-global-error-handler-tutorial-with-example and https://enlear.academy/global-exception-handling-in-net-6-16908fc8dc28
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            switch (exception)
            {
                case ApplicationException ex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case KeyNotFoundException ex:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            logger.LogCritical(exception, "Critical exception.");
            var result = JsonSerializer.Serialize(new { message = exception.Message});
            await context.Response.WriteAsync(result);
        }
    }
}
