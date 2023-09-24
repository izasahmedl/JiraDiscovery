namespace JiraDiscovery.Common.Configuration
{
    public class QuartzJobConfig
    {
        public string JobKey { get; set; } = string.Empty;
        public string CronExp { get; set; } = string.Empty;
        public bool IsDisabled { get; set; }
    }
}
