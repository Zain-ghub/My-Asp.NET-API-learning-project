namespace RepoApi.Middleware
{
    public class ExeptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExeptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try {
                await _next(context);
            }
            catch (Exception ex) { 
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync($"{{\"error\" : \"{ex.Message}\"}}");
            }
            
        }
    }
}
