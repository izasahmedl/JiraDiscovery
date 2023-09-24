using Microsoft.Extensions.Configuration;
using JiraDiscovery.Common.Configuration;
using static JiraDiscovery.Common.Extensions.ServiceCollectionExtensions;
using Microsoft.Extensions.DependencyInjection;
using static JiraDiscovery.Common.Constants.ApplicationConstants;

namespace JiraDiscovery.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class QuartzJobAttribute : Attribute
    {
        private readonly QuartzJobConfig _quartzJobConfig;

        public QuartzJobAttribute(string jobKey)
        {
            var config = GetServiceProvider.GetService<IConfiguration>();

            var jobConfigs = config!.GetSection(CronJobs).Get<List<QuartzJobConfig>>();

            var jobConfig = jobConfigs!.Single(c => c.JobKey == jobKey);

            _quartzJobConfig = jobConfig;
        }

        public QuartzJobConfig QuartzJobConfig
        {
            get { return _quartzJobConfig; }
        }
    }
}
