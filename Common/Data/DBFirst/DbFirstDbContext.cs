using Microsoft.EntityFrameworkCore;

namespace EFExample.Common.Data.DBFirst;

public partial class DbFirstDbContext : DbContext
{
    public DbFirstDbContext() { }

    public DbFirstDbContext(DbContextOptions<DbFirstDbContext> options) : base(options) { }
}
