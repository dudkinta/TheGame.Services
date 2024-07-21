using HuntingDbContext.Models;
using Microsoft.EntityFrameworkCore;

namespace HuntingDbContext
{
    public class HuntingContext : DbContext, IHuntingContext
    {
        public HuntingContext(DbContextOptions<HuntingContext> options)
            : base(options)
        {
        }

        public DbSet<GameModel> Games { get; set; }

        public async Task<int> SaveAsync(CancellationToken cancellationToken)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameModel>().ToTable("games");

            modelBuilder.Entity<GameModel>()
            .HasKey(e => e.game_guid);
        }
    }
}
