using System.Net;
using System.Text.Json;

namespace ProductCatalogueManagement.API.Middleware
{
    public class ExceptionHandlerMiddleware
    {

        private readonly RequestDelegate _next;

        private const string HeaderName = "X-Correlation-ID";

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var correlationId = context.Items[HeaderName]?.ToString();

                if (string.IsNullOrEmpty(correlationId))
                {
                    correlationId = Guid.NewGuid().ToString();
                }

                context.Response.Headers[HeaderName] = correlationId;

                await HandleExceptionAsync(context, correlationId, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, string correlationId, Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            context.Response.ContentType = "application/json";

            var response = new
            {
                message = ex,
                correlationId = correlationId
            };

            var json = JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(json);
        }
    }
}