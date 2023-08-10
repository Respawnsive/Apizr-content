using Microsoft.EntityFrameworkCore;

namespace StarCellar.Api
{
    internal class CellarDbContext : DbContext
    {
        public CellarDbContext(DbContextOptions<CellarDbContext> options)
            : base(options) { }

        public DbSet<Wine> Wines => Set<Wine>();
    }
}