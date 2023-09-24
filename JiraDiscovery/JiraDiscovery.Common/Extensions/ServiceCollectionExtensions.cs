using Microsoft.Extensions.Configuration;

namespace JiraDiscovery.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static IServiceProvider _provider = default!;
        private static IConfiguration _configuration = default!;
        
        public static IServiceProvider GetServiceProvider
        {
            get
            {
                if (_provider != null)
                    return _provider;
                else
                    throw new NullReferenceException($"{nameof(GetServiceProvider)} is null, need to {nameof(CaptureServiceProvider)} reference from host before using it");
            }
        }

        public static void CaptureServiceProvider(this IServiceProvider serviceProvider)
        {
            _provider = serviceProvider;
        }
    }
}
