namespace JiraDiscovery.ExternalService.Interfaces
{
    public interface IJiraTokenService
    {
        Task<string> GenerateOuthTokenAsync();
    }
}
