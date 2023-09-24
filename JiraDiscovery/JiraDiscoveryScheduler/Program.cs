using JiraDiscovery.Common.Configuration;
using JiraDiscovery.Common.Extensions;
using static JiraDiscovery.Common.Extensions.ServiceCollectionExtensions;
using static JiraDiscovery.ExternalService.Extensions.ServiceCollectionExtensions;

namespace JiraDiscoveryScheduler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hb, services) =>
                {
                    services.BuildServiceProvider().CaptureServiceProvider();
                    
                    services.ConfigureQuartzJobs();

                    services.Configure<Jira>(hb.Configuration.GetSection($"{nameof(Jira)}"));

                    services.ConfigureExternalServices(hb);
                })
                .Build();

            host.Run();
        }
    }
}