using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Algar.Hours.Persistence
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }
    }
}
