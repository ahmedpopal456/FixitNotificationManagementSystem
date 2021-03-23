using Fixit.Notification.Management.Lib.Networking.Local;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace Fixit.Notification.Management.Lib.Networking.Extensions
{
    public static class HttpClientCoreExtension
    {
        public static void AddUmServices(this IServiceCollection services, string serviceEndpoint)
        {
            services.AddTransient<IFixItHttpClient, FixItHttpClient>(provider =>
            {
                var httpClientCore = new HttpClientCore(new HttpClient(), serviceEndpoint);
                return new FixItHttpClient(httpClientCore);
            });
        }
    }
}
