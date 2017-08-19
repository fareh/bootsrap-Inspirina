using MedStaffConsult.Storage.Abstraction;
using MedStaffConsult.Web.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MedStaffConsult.Models;
using MedStaffConsult.Storage.Implementation.Front.Core.Storage.KeyDocument;

namespace MedStaffConsult.Web.AppBuilderExtensions
{
    public static class StartupStorageExtentions
    {
        public static IServiceCollection AddStorages(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConfSection = configuration.GetSection("DatabaseConfiguration");
            var dbConf = dbConfSection.Get<DatabaseConfiguration>();

            services.AddSingleton<IRepoStorage<Doctor>>(sc =>
            {
                var logger = sc.GetService<ILogger<IRepoStorage<Doctor>>>();
                return new MySqlRepoStorage<Doctor>(
                    logger,
                    dbConf?.ConnectionString,
                    dbConf?.DbSchema
                );
            });

            return services;
        }
    }
}
