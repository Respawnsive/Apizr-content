using Microsoft.EntityFrameworkCore;

namespace StarCellar.Api.Data
{
    internal class TokenDbContext : DbContext
    {
        public TokenDbContext(DbContextOptions<TokenDbContext> options)
            : base(options) { }

        public DbSet<Token> Tokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Token>().HasIndex(b => b.UserId);
        }
    }
}
