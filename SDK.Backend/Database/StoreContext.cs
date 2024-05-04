
using Microsoft.EntityFrameworkCore;

namespace SDK.Backend.Database
{
    public class StoreContext(DbContextOptions options) : DbContext(options)
    {
    }
}
