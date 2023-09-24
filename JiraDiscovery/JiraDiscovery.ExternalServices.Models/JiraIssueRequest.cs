namespace JiraDiscovery.ExternalServices.Models
{
    public class JiraIssueRequest
    {
        public List<string> Expand { get; set; } = new List<string>();

        public List<string> Fields { get; set; } = new List<string>();

        public bool FieldsByKeys { get; set; } = false;

        public string Jql { get; set; } = string.Empty;

        public int MaxResults { get; set; }

        public int StartAt { get; set; }
    }
}