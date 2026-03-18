namespace ProductCatalogueManagement.API.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        private const string HeaderName = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(HeaderName, out var correlationId))

                correlationId = Guid.NewGuid().ToString();

            context.Response.Headers[HeaderName] = correlationId;

            await _next(context);
        }
    }
}
