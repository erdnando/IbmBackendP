using Algar.Hours.Application.DataBase;
using Algar.Hours.Persistence.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Algar.Hours.External
{
    public static class DependencyInjectionService
    {
        public static IServiceCollection AddExternal(this IServiceCollection services, IConfiguration configuration)
        {

            //services.AddDbContext<DatabaseService>(options =>
            //options.UseSqlServer(configuration["sqlconnectionstrings"]));

            // services.AddDbContext<DatabaseService>(options =>
            //options.UseNpgsql(configuration["SQLConnectionStringsPost"]));

            /* services.AddDbContext<DatabaseService>(options =>
             {
                 options.UseNpgsql(configuration["SQLConnectionStringsPost"],
                     providerOptions =>
                     {
                         providerOptions
                             .EnableRetryOnFailure(
                                 maxRetryCount: 5,
                                 maxRetryDelay: TimeSpan.FromSeconds(30),
                                 errorCodesToAdd:null
                                 )
                             .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                     });
                 options.EnableSensitiveDataLogging();
                 options.ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
             });
             */


            services.AddDbContext<DatabaseService>(options => options.UseNpgsql(configuration["SQLConnectionStringsPost"],providerOptions => providerOptions.EnableRetryOnFailure() ));

            services.AddTransient<DatabaseService>();

            services.AddScoped<IDataBaseService, DatabaseService>();

            return services;
        }
    }
}
