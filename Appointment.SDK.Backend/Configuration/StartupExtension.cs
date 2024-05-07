
using Appointment.SDK.Backend.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Appointment.SDK.Backend.Configuration
{
    public static class StartupExtension
    {
        public static void AddGlobalConfiguration<T>(this IServiceCollection services, IConfigurationManager configurationManager) where T : StoreContext
        {
            var ConnectionString = configurationManager.GetConnectionString("ConnectionString")!;

            services.AddDbContextFactory<T>((sp, opt) => DbContextOptions(sp, opt, ConnectionString), ServiceLifetime.Transient);
        }

        static void DbContextOptions(IServiceProvider sp, DbContextOptionsBuilder options, string ConnectionString)
        {
            var DbInstance = (IDatabase) ActivatorUtilities.CreateInstance(sp, typeof(Postgresql), options, ConnectionString);
            DbInstance.SetConnection();
        }
    }
}
