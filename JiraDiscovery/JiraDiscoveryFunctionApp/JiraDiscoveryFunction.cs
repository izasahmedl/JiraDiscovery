using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using JiraDiscovery.ExternalService.Interfaces;
using static JiraDiscovery.Common.Constants.ApplicationConstants.Jira;
using System.Linq;

namespace JiraDiscoveryFunctionApp
{
    public class JiraDiscoveryFunction
    {
        private readonly IJiraService _jiraService;

        public JiraDiscoveryFunction(IJiraService jiraService)
        {
            _jiraService = jiraService;
        }

        [FunctionName("JiraDiscoveryFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var response = await _jiraService.SearchIssuesAsync(SearchIssueJQLQuery).ConfigureAwait(false);

            return new OkObjectResult(string.Join(',',response));
        }
    }
}
