using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OfficeAssetManager.Core.Domain.RepositoryContracts;
using OfficeAssetManager.Infrastructure.DbContext;
using OfficeAssetManager.Infrastructure.Repositories;

namespace OfficeAssetManager.Infrastructure
{

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["DB_CONNECTION_STRING"];

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DB_CONNECTION_STRING' not found in configuration.");
            }

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<IAssetRepository,AssetRepository>();

            services.AddScoped<IAssetLogRepository,AssetLogRepository>();

            services.AddScoped<IReservationRepository,ReservationRepository>();

            return services;
        }
    }
}