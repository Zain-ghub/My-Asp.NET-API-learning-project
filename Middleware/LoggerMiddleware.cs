namespace RepoApi.Middleware
{
    public class LoggerMiddleware
    {
        private readonly ILogger<LoggerMiddleware> _logger;

        private readonly RequestDelegate _next;

        public LoggerMiddleware(ILogger<LoggerMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context) 
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation($"{context.Request.Method} {context.Request.Path}");
            await _next(context);
            sw.Stop();
            _logger.LogInformation($"{context.Request.Method} {context.Request.Path} - {context.Response.StatusCode} Time Elapsed - {sw.ElapsedMilliseconds}ms");
        
        }
    }
}
