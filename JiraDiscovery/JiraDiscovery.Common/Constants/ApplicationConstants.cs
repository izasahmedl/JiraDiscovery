
namespace JiraDiscovery.Common.Constants
{
    public static class ApplicationConstants
    {
        public const string CronJobs = "CronJobs";

        public static class Jira
        {
            public const string SearchIssueJQLQuery = "project = JSWCLOUD AND issuetype = Bug AND status = Closed AND resolution = Fixed ORDER BY priority DESC, updated DESC";

            public const int MaxResultsPerPage = 100;
        }

        public static class Endpoints
        {
            public static class Jira
            {
                public const string SearchIssues = "/rest/api/2/search";
            }
        }
    }
}
