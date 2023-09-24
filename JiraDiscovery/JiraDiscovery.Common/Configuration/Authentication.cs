namespace JiraDiscovery.Common.Configuration
{
    public class Authentication
    {
        public string TokenGenerationUrl { get; set; } = string.Empty;

        public string ClientId { get; set; } = string.Empty;

        public string ClientSecret { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string APIToken { get; set; } = string.Empty;
    }
}
