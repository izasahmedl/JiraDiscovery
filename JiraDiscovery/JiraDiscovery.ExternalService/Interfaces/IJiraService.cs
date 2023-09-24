namespace JiraDiscovery.ExternalService.Interfaces
{
    public interface IJiraService 
    {
        Task<List<string>> SearchIssuesAsync(string jqlQuery);
    }
}
