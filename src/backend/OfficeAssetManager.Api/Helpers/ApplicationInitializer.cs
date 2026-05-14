using Microsoft.AspNetCore.Identity;
using OfficeAssetManager.Core.Domain.Entities;

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
    }
}
