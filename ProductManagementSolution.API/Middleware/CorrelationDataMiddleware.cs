namespace ProductCatalogueManagement.API.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        private const string HeaderName = "X-Correlation-ID";

        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(HeaderName, out var correlationId))

                correlationId = Guid.NewGuid().ToString();

            context.Items[HeaderName] = correlationId.ToString();

            context.Response.Headers[HeaderName] = correlationId;

            _logger.LogInformation("Incoming Request: {Method} {Path} | CorrelationId: {CorrelationId}", context.Request.Method, context.Request.Path, correlationId);

            await _next(context);

            _logger.LogInformation("Outgoing Response: {StatusCode} | CorrelationId: {CorrelationId}", context.Response.StatusCode, correlationId);
        }
    }
}
