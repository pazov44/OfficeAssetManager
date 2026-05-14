namespace OfficeAssetManager.Api.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseApiMiddleware(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            return app;
        }
    }
}
