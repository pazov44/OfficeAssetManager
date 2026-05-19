using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeAssetManager.Core;
using OfficeAssetManager.Core.Configuration;
using OfficeAssetManager.Core.Domain.Entities;
using OfficeAssetManager.Infrastructure;
using OfficeAssetManager.Infrastructure.DbContext;
using System.Text;

namespace OfficeAssetManager.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddInfrastructure(configuration);
            services.AddCore();

            services.AddApiCors(configuration);
            services.AddApiIdentityAndAuth(configuration);
            services.AddApiSwagger();

            return services;
        }

        private static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });

            var corsOptions = new CorsOptions();
            configuration.GetSection("CorsSettings").Bind(corsOptions);
            services.AddSingleton(corsOptions);

            services.AddCors(options =>
            {
                string policyName = corsOptions.PolicyName
                    ?? throw new InvalidOperationException("CORS Policy Name is missing!");

                var allowedOrigins = corsOptions.AllowedOrigins ?? Array.Empty<string>();

                options.AddPolicy(name: policyName,
                    policy =>
                    {
                        policy.WithOrigins(allowedOrigins)
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            return services;
        }
        private static IServiceCollection AddApiIdentityAndAuth(this IServiceCollection services, IConfiguration configuration)
        {
            string jwtSecretKey = configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Secret Key is missing!");

            var jwtOptions = new JwtOptions();
            configuration.GetSection("Jwt").Bind(jwtOptions);
            jwtOptions.SecretKey = jwtSecretKey;
            services.AddSingleton(jwtOptions);

            // ASP.NET Core Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // JWT Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtOptions.SecretKey))
                };
            });

            services.AddAuthorization();

            // Global Authorization Filter for Controllers
            services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            });

            return services;
        }
        private static IServiceCollection AddApiSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Office Asset Manager API", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] {}
                    }
                });
            });

            return services;
        }

    }
}