

using Microsoft.EntityFrameworkCore;

namespace Appointment.SDK.Backend.Database
{
    public class Postgresql(IServiceProvider serviceProvider, DbContextOptionsBuilder dbContextOptions, string Connection) : Database(serviceProvider, dbContextOptions, Connection)
    {
        public override void SetConnection()
        {
            dbContextOptions.UseNpgsql(ConnectionString);
        }
    }
}