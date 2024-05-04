
using Microsoft.EntityFrameworkCore;

namespace Appointment.SDK.Backend.Database
{
    public class StoreContext(DbContextOptions options) : DbContext(options)
    {
    }
}
