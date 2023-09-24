using JiraDiscovery.Common.Configuration;
using JiraDiscovery.ExternalService.Implementations;
using JiraDiscovery.ExternalService.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace JiraDiscovery.ExternalService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureExternalServices(this IServiceCollection services, HostBuilderContext ctx)
        {
            services.AddHttpClient<IJiraService, JiraService>(httpClient =>
            {
                var authenticationConfig = ctx.Configuration.GetSection($"{nameof(Jira)}").Get<Jira>() ?? throw new InvalidOperationException($"Configuration section {nameof(Jira)} cannot be empty");

                var tokenUrl = authenticationConfig.BaseUrl ?? throw new InvalidOperationException($"Base url config section {nameof(Jira)}:{nameof(Jira.BaseUrl)} cannot be empty");

                httpClient.BaseAddress = new Uri(tokenUrl);

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            });

            return services;
        }

        public static IServiceCollection ConfigureExternalServicesForFunctionApp(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IJiraService, JiraService>(httpClient =>
            {
                var url = configuration["Jira:BaseUrl"];

                httpClient.BaseAddress = new Uri(url);

                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            });

            return services;
        }
    }
}
