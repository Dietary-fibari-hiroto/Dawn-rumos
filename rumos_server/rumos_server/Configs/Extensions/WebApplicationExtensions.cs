using rumos_server.Configs.Middlewares;

namespace rumos_server.Configs.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseHsts();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.UseCors("MauiApp");
            app.UseMiddleware<ApiKeyAuthMiddleware>();//ミドルウェアの登録(API)

            return app;
        }

        public static WebApplication MapEndpoints(this WebApplication app)
        {
            app.MapGet("/", () => "Dawn-Rumos");
            app.MapControllers().RequireRateLimiting("api");

            return app;
        }
    }
}


