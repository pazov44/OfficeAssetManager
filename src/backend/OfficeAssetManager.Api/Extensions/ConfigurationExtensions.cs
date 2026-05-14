using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;

namespace OfficeAssetManager.Api.Extensions
{
    public static class CollectionExtensions
    {
        public static IConfigurationBuilder AddEnvConfig(this IConfigurationBuilder configuration)
        {
            var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());

            int countDir = 0;

            // Loop upwards until we reach the solution files or parents are more than 5
            while (currentDir != null && countDir < 5)
            {
                var files = currentDir.GetFiles("*.env*");

                if (files.Any())
                {
                    foreach (var file in files)
                    {
                        DotNetEnv.Env.Load(file.FullName);
                    }
                }

                if (currentDir.GetFiles("*.sln").Any() || currentDir.GetFiles("*.slnx").Any()) break;

                currentDir = currentDir.Parent;
                countDir++;
            }

            // Sync with System Environment Variables (Docker/Production)
            configuration.AddEnvironmentVariables();

            return configuration;
        }
    }
}