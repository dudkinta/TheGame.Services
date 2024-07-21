using Microsoft.EntityFrameworkCore;
using StatisticDbContext.Models;

namespace StatisticDbContext
{
    public class StatisticContext : DbContext, IStatisticContext
    {
        public DbSet<StorageModel> Storage { get; set; }
        public StatisticContext(DbContextOptions<StatisticContext> options)
            : base(options)
        {
        }

        public async Task<int> SaveAsync(CancellationToken cancellationToken)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StorageModel>().ToTable("storage");

            modelBuilder.Entity<StorageModel>()
            .HasKey(e => e.id);
        }
    }
}
