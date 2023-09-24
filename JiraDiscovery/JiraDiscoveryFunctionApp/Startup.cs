using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using JiraDiscovery.Common.Configuration;
using JiraDiscovery.Common.Extensions;
using static JiraDiscovery.Common.Extensions.ServiceCollectionExtensions;
using static JiraDiscovery.ExternalService.Extensions.ServiceCollectionExtensions;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;

[assembly: FunctionsStartup(typeof(JiraDiscoveryFunctionApp.Startup))]
namespace JiraDiscoveryFunctionApp
{
    public class Startup : FunctionsStartup
    {
        private static IConfiguration _configuration;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Configure your services here
            //builder.Services.AddSingleton<IMyService, MyService>();
            // Add other services as needed

            var services = builder.Services;

            var configurationBuilder = BuildConfigurationBuilder(builder);

            _configuration = configurationBuilder.Build();

            services.BuildServiceProvider().CaptureServiceProvider();

            services.Configure<Jira>(_configuration.GetSection($"{nameof(Jira)}"));

            services.ConfigureExternalServices(_configuration);
        }

        private static IConfigurationBuilder BuildConfigurationBuilder(IFunctionsHostBuilder hostBuilder)
        {
            try
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());

                var serviceProvider = hostBuilder.Services.BuildServiceProvider();

                var existingConfigInstance = serviceProvider.GetService<IConfiguration>();

                if (existingConfigInstance != null)
                {
                    builder.AddConfiguration(existingConfigInstance);
                }

                var customEnvName = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile("appsettings." + customEnvName + ".json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();

                return builder;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
