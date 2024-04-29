using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Common.StartUp
{
    public static class ServiceHelper
    {

        static IServiceProvider services = null;

        public static IServiceProvider Services
        {
            get { return services; }

            set
            {
                if (services != null)
                {
                    throw new Exception("Can't set once a value has already been set");
                }
                services = value;
            }
        }

        public static IDistributedCache DistributedCache
        {
            get
            {
                return services.GetService(typeof(IDistributedCache)) as IDistributedCache;
            }
        }
    }
}
