using JiraDiscovery.Common.Attributes;
using JiraDiscovery.ExternalService.Interfaces;
using Quartz;
using static JiraDiscovery.Common.Constants.ApplicationConstants.Jira;

namespace JiraDiscoveryScheduler.Jobs
{
    [QuartzJob(nameof(JiraDiscoveryJob))]
    public class JiraDiscoveryJob : IJob
    {
        private readonly ILogger<JiraDiscoveryJob> _logger;
        private readonly IJiraService _jiraService;

        public JiraDiscoveryJob(ILogger<JiraDiscoveryJob> logger, IJiraService jiraService)
        {
            _logger = logger;
            _jiraService = jiraService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Executed at current time {time}", DateTime.Now);

            var response = await _jiraService.SearchIssuesAsync(SearchIssueJQLQuery).ConfigureAwait(false);

            Console.WriteLine(string.Join(',', response));
        }
    }
}
