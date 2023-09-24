using System.Reflection;
using static JiraDiscovery.Common.Constants.AssemblyConstants;

namespace JiraDiscovery.Common.Extensions
{
    public static class AssemblyExtensions
    {
        public static Assembly GetSchedulerAssembly(this AppDomain appDomain)
        {
            return appDomain.GetAssembly(SchedulerAssemblyName);
        }

        private static Assembly GetAssembly(this AppDomain appDomain, string assemblyName)
        {
            return appDomain
               .GetAssemblies()
               .Single(a => a.GetName().Name == assemblyName);
        }
    }
}
