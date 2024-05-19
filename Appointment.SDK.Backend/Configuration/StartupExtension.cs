
using Appointment.SDK.Backend.Database;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
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

            services.AddTransient(typeof(IDbContextFactory<StoreContext>), x => {
                var RealContextFactory = typeof(IDbContextFactory<>)
                    .MakeGenericType(typeof(T));
                var Factory = x.GetRequiredService(RealContextFactory);
                return Factory;
            });
        }

        public static void UseGoogleAuthentication(this IServiceCollection services, IConfigurationManager configurationManager)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                })
                .AddCookie(opt =>{
                    opt.ExpireTimeSpan = TimeSpan.FromMinutes(120);
                })
                .AddGoogle(options =>
                {
                    options.ClientId = configurationManager["GoogleKeys:ClientId"]!;
                    options.ClientSecret = configurationManager["GoogleKeys:SecretId"]!;
                });
        }

        public static void DbContextOptions(IServiceProvider sp, DbContextOptionsBuilder options, string ConnectionString)
        {
            var DbInstance = (IDatabase) ActivatorUtilities.CreateInstance(sp, typeof(Postgresql), options, ConnectionString);
            DbInstance.SetConnection();
        }
    }
}
