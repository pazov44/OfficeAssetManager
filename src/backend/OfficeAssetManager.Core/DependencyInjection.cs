using Microsoft.Extensions.DependencyInjection;
using OfficeAssetManager.Core.ServiceContracts;
using OfficeAssetManager.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAssetManager.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddScoped<IJwtService, JwtService>();

            services.AddScoped<IAuthService,AuthService>();

            services.AddScoped<IAssetService,AssetService>();

            return services;
        }
    }
}
