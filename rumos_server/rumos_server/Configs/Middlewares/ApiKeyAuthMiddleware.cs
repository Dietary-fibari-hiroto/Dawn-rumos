namespace rumos_server.Configs.Middlewares
{
    public class ApiKeyAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _apiKey;

        public ApiKeyAuthMiddleware(RequestDelegate next)
        {
            _next = next;
            _apiKey = Environment.GetEnvironmentVariable("RUMOS_API_KEY");

            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new InvalidOperationException("RUMOSのAPIキーが見つからないため、システムの起動を中断します。");
            }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if(!context.Request.Headers.TryGetValue("X-API-Key",out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API_KEYが見つかりません。");
                return;
            }

            if (!extractedApiKey.Equals(_apiKey)){
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("APIキーが違います。");
                return;
            }

            await _next(context);
        }


    }
}
