namespace JiraDiscovery.Common.Configuration
{
    public class Jira
    {
        public Authentication Authentication { get; set; } = new Authentication();

        public string BaseUrl { get; set; } = string.Empty;

        public string CloudUrlEndpoint { get; set; } = string.Empty;
    }
}
