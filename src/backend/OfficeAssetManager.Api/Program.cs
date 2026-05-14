using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Writers;
using OfficeAssetManager.Api.Extensions;
using OfficeAssetManager.Api.Helpers;
using OfficeAssetManager.Core.Domain.Entities;

namespace OfficeAssetManager.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddEnvConfig();

            builder.Services.AddApiServices(builder.Configuration);

            var app = builder.Build();

            await ApplicationInitializer.Initialize(app.Services);

            app.UseApiMiddleware();

            app.Run();
        }
    }
}

