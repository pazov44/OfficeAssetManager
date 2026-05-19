using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeAssetManager.Core.Domain.Entities;
using OfficeAssetManager.Infrastructure.DbContext;

namespace OfficeAssetManager.Api.Helpers
{
    /// <summary>
    /// Provides methods to initialize application-level data and settings during startup.
    /// </summary>
    public static class ApplicationInitializer
    {
        /// <summary>
        /// Orchestrates all startup initialization tasks.
        /// </summary>
        /// <param name="services">The root service provider from the WebApplication.</param>
        public static async Task Initialize(IServiceProvider services)
        {
            await ApplyDatabaseMigrations(services);

            await InitializeIdentityRoles(services);
        }

        /// <summary>
        /// Ensures that the required Identity roles exist in the database.
        /// </summary>
        /// <param name="services">The service provider used to create a scope for resolving Identity services.</param>
        private static async Task InitializeIdentityRoles(IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                string[] roleNames = { "User", "Admin" };

                foreach (var roleName in roleNames)
                {
                    var exists = await roleManager.RoleExistsAsync(roleName);

                    if(!exists)
                    {
                        await roleManager.CreateAsync(new ApplicationRole()
                        {
                            Name = roleName
                        });
                    }
                }
            }
        }

        private static async Task ApplyDatabaseMigrations(IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {
                    var context = serviceProvider.GetRequiredService<AppDbContext>();
                    Console.WriteLine("Checking for pending database migrations...");

                    await context.Database.MigrateAsync();

                    Console.WriteLine("Database migrations applied successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred during database migration: {ex.Message}");
                    throw; // Rethrow so the app stops if the DB setup fails
                }
            }
        }
    }
}
