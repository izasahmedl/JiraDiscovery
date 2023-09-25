using JiraDiscovery.ExternalService.Interfaces;
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
using Microsoft.Extensions.Configuration;

namespace JiraDiscovery.ExternalService.Implementations
{
    public class JiraService : IJiraService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<JiraService> _logger;
        private readonly IConfiguration _configuration;

        public JiraService(HttpClient httpClient, ILogger<JiraService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<List<string>> SearchIssuesAsync(string jqlQuery)
        {
            var issues = new List<string>();
            
            //Commenting as of now for an anonymous use case
            //var byteArray = Encoding.ASCII.GetBytes($"{_configuration["Jira:Authentication:UserName"]}:{_configuration["Jira:Authentication:APIToken"]}");

            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Basic, Convert.ToBase64String(byteArray));
            
            var startAt = 0; var maxResults = 100;  bool hasMoreData = true;

            var url = $"{SearchIssues}";

            while (hasMoreData)
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url);

                var jiraRequest = new JiraIssueRequest {
                    Expand = new List<string>() { JiraIssueExpandEnum.Operations.ToString().ToLower(), JiraIssueExpandEnum.Names.ToString().ToLower(), JiraIssueExpandEnum.Schema.ToString().ToLower() },
                    Fields = new List<string> { JiraIssueFiltersEnum.Summary.ToString().ToLower(), JiraIssueFiltersEnum.Assignee.ToString().ToLower() , JiraIssueFiltersEnum.Status.ToString().ToLower() },
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

                    return new List<string>();
                }

                foreach (var issue in issueJson["issues"]!)
                {
                    var issueKey = issue["key"]?.ToString() ?? throw new InvalidOperationException("Issue key Response cannot be null");

                    _logger.LogInformation("{MACHINE_NAME} - Writing issue {ISSUE_KEY} details in json.", Environment.MachineName, issueKey);

                    Console.Write(issueKey);

                    _logger.LogInformation("{MACHINE_NAME} - Wrote issue {ISSUE_KEY} details in json.", Environment.MachineName, issueKey);

                    issues.Add(issueKey);
                }
                
                var total = issueJson["total"]?.Value<int>();

                _logger.LogInformation("{MACHINE_NAME} - Number of issues processed in this request {TOTAL}", Environment.MachineName, total);

                startAt += MaxResultsPerPage;

                hasMoreData = false; //Kept for time being

                if (startAt >= total)
                {
                    _logger.LogInformation("{MACHINE_NAME} - No more issues from JIRA to process", Environment.MachineName);

                    hasMoreData = false;
                }
            }
            return issues;
        }
    }
}
