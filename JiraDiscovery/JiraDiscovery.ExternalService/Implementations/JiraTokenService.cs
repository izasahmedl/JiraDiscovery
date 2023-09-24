using JiraDiscovery.Common.Configuration;
using JiraDiscovery.ExternalService.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using static JiraDiscovery.Common.Constants.HttpConstants;

namespace JiraDiscovery.ExternalService.Implementations
{
    public class JiraTokenService: IJiraTokenService
    {
        private readonly HttpClient _httpClient;
        private readonly Authentication _authentication;
        private readonly ILogger<JiraTokenService> _logger;

        public JiraTokenService(HttpClient httpClient, IOptions<Authentication> authentication, ILogger<JiraTokenService> logger)
        {
            _httpClient = httpClient;
            _authentication = authentication.Value;
            _logger = logger;
        }

        public async Task<string> GenerateOuthTokenAsync()
        {
            if (string.IsNullOrWhiteSpace(_authentication.ClientId) || string.IsNullOrWhiteSpace(_authentication.ClientSecret))
                throw new InvalidOperationException($"Jira token generation configuration is invalid.");

            var tokenObject = await GenerateServiceTokenAsync("", _authentication.ClientId, _authentication.ClientSecret);

            if (string.IsNullOrEmpty(tokenObject))
                throw new InvalidOperationException($"Token object response cannot be null or empty.");

            var tokenJObject = JObject.Parse(tokenObject);
             
            var accessToken = Convert.ToString(tokenJObject.SelectToken("$.access_token")) ?? string.Empty;

            if (string.IsNullOrEmpty(accessToken))
                throw new InvalidOperationException($"Jira Token cannot be null or empty.");

            return accessToken;
        }

        private async Task<string> GenerateServiceTokenAsync(string tokenGenerationUri, string clientId, string clientSecret)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, tokenGenerationUri);

            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("grant_type", ClientCredentials)
            };

            request.Content = new FormUrlEncodedContent(keyValues);

            HttpResponseMessage response;

            _logger.LogInformation("{MACHINE_NAME} - Token is not cached. Hence generating one.", Environment.MachineName);

            response = await _httpClient.SendAsync(request, CancellationToken.None).ConfigureAwait(false);

            var tokenResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            _logger.LogInformation("{MACHINE_NAME} - Generated token successfully.", Environment.MachineName);

            return tokenResponse;
        }
    }
}
