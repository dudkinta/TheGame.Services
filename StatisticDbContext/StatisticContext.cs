using Microsoft.EntityFrameworkCore;
using StatisticDbContext.Models;

namespace StatisticDbContext
{
    public class StatisticContext : DbContext, IStatisticContext
    {
        public DbSet<StorageModel> Storage { get; set; }
        public DbSet<InventoryModel> Inventory { get; set; }
        public DbSet<ItemModel> Items { get; set; }

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
            modelBuilder.Entity<StorageModel>(entity =>
            {
                entity.ToTable("storage");
                entity.HasKey(e => e.user_id);
            }
            );

            modelBuilder.Entity<ItemModel>(entity =>
            {
                entity.ToTable("items");
                entity.HasKey(e => e.id);
                entity.Property(e => e.name)
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(e => e.description)
                    .IsRequired();
                entity.Property(e => e.type)
                    .HasMaxLength(50)
                    .IsRequired();
                entity.Property(e => e.level)
                    .IsRequired();
                entity.Property(e => e.asset)
                    .HasMaxLength(255)
                    .IsRequired();
            });

            modelBuilder.Entity<InventoryModel>(entity =>
            {
                entity.ToTable("inventory");
                entity.HasKey(e => e.id);
                entity.Property(e => e.user_id)
                    .IsRequired();
                entity.Property(e => e.item_id)
                    .IsRequired();
                entity.Property(e => e.count)
                    .IsRequired();
                entity.HasOne(e => e.item)
                    .WithMany()
                    .HasForeignKey(e => e.item_id);
            });
        }
    }
}
