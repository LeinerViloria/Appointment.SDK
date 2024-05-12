
using Appointment.SDK.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appointment.SDK.Backend.Database
{
    public class StoreContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Company> Companies { get; set; }
    }
}
