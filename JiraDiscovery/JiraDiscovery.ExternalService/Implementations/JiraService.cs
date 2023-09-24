using JiraDiscovery.Common.Configuration;
using JiraDiscovery.ExternalService.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using static JiraDiscovery.Common.Constants.HttpConstants;
using static JiraDiscovery.Common.Constants.ApplicationConstants.Endpoints.Jira;
using static JiraDiscovery.Common.Constants.ApplicationConstants.Jira;
using JiraDiscovery.ExternalServices.Models;
using System.Text.Json;
using System.Net.Mime;
using JiraDiscovery.ExternalServices.Models.Enums;
using Microsoft.Extensions.Logging;

namespace JiraDiscovery.ExternalService.Implementations
{
    public class JiraService : IJiraService
    {
        private readonly HttpClient _httpClient;
        private readonly Jira _jira;
        private readonly ILogger<JiraService> _logger;
        private readonly Authentication _authentication;

        public JiraService(HttpClient httpClient, IOptions<Authentication> authentication, IOptions<Jira> jira, ILogger<JiraService> logger)
        {
            _httpClient = httpClient;
            _jira = jira.Value;
            _logger = logger;
            _authentication = authentication.Value;
        }

        public async Task SearchIssuesAsync(string jqlQuery)
        {
            var byteArray = Encoding.ASCII.GetBytes($"{_authentication.UserName}:{_authentication.APIToken}");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Basic, Convert.ToBase64String(byteArray));
            
            var startAt = 0; var maxResults = 100;  bool hasMoreData = true;

            var url = $"{_jira.CloudUrlEndpoint}{SearchIssues}";

            while (hasMoreData)
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url);

                var jiraRequest = new JiraIssueRequest {
                    Expand = new List<string>() { JiraIssueExpandEnum.Operations.ToString().ToLower(), JiraIssueExpandEnum.Names.ToString().ToLower(), JiraIssueExpandEnum.Schema.ToString().ToLower() },
                    Fields = new List<string> { JiraIssueFiltersEnum.Summary.ToString().ToLower(), JiraIssueFiltersEnum.Assignee.ToString().ToLower() , JiraIssueFiltersEnum.Status.ToString().ToLower() },
                    FieldsByKeys = false,
                    Jql = jqlQuery,
                    MaxResults = maxResults,
                    StartAt = startAt,
                };
                
                request.Content = new StringContent(JsonSerializer.Serialize(jiraRequest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase}),Encoding.UTF8, MediaTypeNames.Application.Json);

                var response = await _httpClient.SendAsync(request);

                var issueResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                _logger.LogInformation("{MACHINE_NAME} - Received issues from JIRA.", Environment.MachineName);

                var issueJson = JObject.Parse(issueResponse) ?? throw new InvalidOperationException("Search issues json Response cannot be null");

                if(!issueJson.ContainsKey("issues"))
                {
                    _logger.LogInformation("{MACHINE_NAME} - No Issues found. Hence coming out of the loop.", Environment.MachineName);

                    return;
                }

                foreach (var issue in issueJson["issues"]!)
                {
                    var issueKey = issue["key"]?.ToString() ?? throw new InvalidOperationException("Issue key Response cannot be null");

                    _logger.LogInformation("{MACHINE_NAME} - Writing issue {ISSUE_KEY} details in json.", Environment.MachineName, issueKey);

                    File.WriteAllText($"{issueKey}.json", issue.ToString());

                    _logger.LogInformation("{MACHINE_NAME} - Wrote issue {ISSUE_KEY} details in json.", Environment.MachineName, issueKey);
                }
                
                var total = issueJson["total"]?.Value<int>();

                _logger.LogInformation("{MACHINE_NAME} - Number of issues processed in this request {TOTAL}", Environment.MachineName, total);

                startAt += MaxResultsPerPage;

                if (startAt >= total)
                {
                    _logger.LogInformation("{MACHINE_NAME} - No more issues from JIRA to process", Environment.MachineName);

                    hasMoreData = false;
                }
            }
        }
    }
}
