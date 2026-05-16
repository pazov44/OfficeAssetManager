namespace OfficeAssetManager.Api.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseApiMiddleware(this WebApplication app, IConfiguration configuration)
        {
            string policyName = configuration["CorsSettings:PolicyName"]
        ?? throw new InvalidOperationException("CORS Policy Name is missing!");

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

            app.UseCors(policyName);

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            return app;
        }
    }
}
