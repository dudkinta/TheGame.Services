using Microsoft.EntityFrameworkCore;
using StatisticDbContext.Models;

namespace StatisticDbContext
{
    public class StatisticContext : DbContext, IStatisticContext
    {
        public DbSet<StorageModel> Storage { get; set; }
        public DbSet<ItemModel> Items { get; set; }
        public DbSet<HeroModel> Heroes { get; set; }
        public DbSet<InventoryModel> Inventory { get; set; }
        public DbSet<BarrackModel> Barracks { get; set; }
        public DbSet<ArmyModel> Armies { get; set; }

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

                entity.Property(e => e.name).HasMaxLength(100);
                entity.Property(e => e.description).HasMaxLength(500);
                entity.Property(e => e.type).HasMaxLength(50);
                entity.Property(e => e.level).IsRequired();
                entity.Property(e => e.asset).HasMaxLength(200);
            });

            modelBuilder.Entity<HeroModel>(entity =>
            {
                entity.ToTable("heroes");
                entity.HasKey(e => e.id);

                entity.Property(e => e.name).HasMaxLength(100);
                entity.Property(e => e.description).HasMaxLength(500);
                entity.Property(e => e.type).HasMaxLength(50);
                entity.Property(e => e.level).IsRequired();
                entity.Property(e => e.asset).HasMaxLength(200);
            });

            modelBuilder.Entity<InventoryModel>(entity =>
            {
                entity.ToTable("inventory");
                entity.HasKey(e => e.id);

                entity.Property(e => e.user_id).IsRequired();
                entity.Property(e => e.item_id).IsRequired();
                entity.Property(e => e.army_id);

                entity.HasOne(d => d.item)
                      .WithMany()
                      .HasForeignKey(d => d.item_id);

                entity.HasOne(d => d.army)
                      .WithMany(a => a.equip)
                      .HasForeignKey(d => d.army_id);
            });

            modelBuilder.Entity<BarrackModel>(entity =>
            {
                entity.ToTable("barracks");
                entity.HasKey(e => e.id);

                entity.Property(e => e.user_id).IsRequired();
                entity.Property(e => e.hero_id).IsRequired();

                entity.HasOne(d => d.hero)
                      .WithMany()
                      .HasForeignKey(d => d.hero_id);

                entity.HasOne(d => d.army)
                      .WithOne(a => a.barrack)
                      .HasForeignKey<ArmyModel>(d => d.barrack_id);
            });

            modelBuilder.Entity<ArmyModel>(entity =>
            {
                entity.ToTable("armies");
                entity.HasKey(e => e.id);

                entity.Property(e => e.user_id).IsRequired();
                entity.Property(e => e.barrack_id).IsRequired();

                entity.HasOne(d => d.barrack)
                      .WithOne(b => b.army)
                      .HasForeignKey<ArmyModel>(d => d.barrack_id);

                entity.HasMany(d => d.equip)
                      .WithOne(i => i.army)
                      .HasForeignKey(i => i.army_id);
            });
        }
    }
}
