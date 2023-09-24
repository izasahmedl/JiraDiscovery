namespace JiraDiscovery.ExternalService.Interfaces
{
    public interface IJiraService 
    {
        Task SearchIssuesAsync(string jqlQuery);
    }
}
