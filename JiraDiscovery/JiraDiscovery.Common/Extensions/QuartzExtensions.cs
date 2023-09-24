using Microsoft.Extensions.DependencyInjection;
using Quartz;
using JiraDiscovery.Common.Helpers;

namespace JiraDiscovery.Common.Extensions
{
    public static class QuartzExtensions
    {
        public static IServiceCollection ConfigureQuartzJobs(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddQuartz(q =>
            {
                serviceCollection.AddQuartz(q =>
                {
                    q.AddQuartzJobsWithTriggers();
                });

                serviceCollection.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            });

            return serviceCollection;
        }
    }
}
