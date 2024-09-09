using Microsoft.EntityFrameworkCore;
using StatisticDbContext.Models;
using StatisticDbContext.Seeds;

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
        public DbSet<RecipeModel> Recipes { get; set; }
        public DbSet<RecipeIngredientModel> RecipeIngredients { get; set; }
        public DbSet<CraftModel> Crafts { get; set; }

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

                entity.HasOne(e => e.item)
                      .WithMany()
                      .HasForeignKey(e => e.item_id);

                entity.HasOne(e => e.army)
                      .WithMany(e => e.equip)
                      .HasForeignKey(e => e.army_id);
            });

            modelBuilder.Entity<BarrackModel>(entity =>
            {
                entity.ToTable("barracks");
                entity.HasKey(e => e.id);

                entity.Property(e => e.user_id).IsRequired();
                entity.Property(e => e.hero_id).IsRequired();

                entity.HasOne(e => e.hero)
                      .WithMany()
                      .HasForeignKey(e => e.hero_id);

                entity.HasOne(e => e.army)
                      .WithOne(e => e.barrack)
                      .HasForeignKey<ArmyModel>(e => e.barrack_id);
            });

            modelBuilder.Entity<ArmyModel>(entity =>
            {
                entity.ToTable("armies");
                entity.HasKey(e => e.id);

                entity.Property(e => e.user_id).IsRequired();
                entity.Property(e => e.barrack_id).IsRequired();

                entity.HasOne(e => e.barrack)
                      .WithOne(e => e.army)
                      .HasForeignKey<ArmyModel>(e => e.barrack_id);

                entity.HasMany(e => e.equip)
                      .WithOne(e => e.army)
                      .HasForeignKey(e => e.army_id);
            });

            modelBuilder.Entity<RecipeModel>(entity =>
            {
                entity.ToTable("recipes");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
                entity.Property(e => e.item_id).HasColumnName("item_id").IsRequired();
                entity.Property(e => e.craft_time).HasColumnName("craft_time").IsRequired();
                entity.Property(e => e.crafting_station).HasColumnName("crafting_station").HasMaxLength(255).IsRequired();
                entity.HasOne(e => e.item)
                      .WithMany()
                      .HasForeignKey(e => e.item_id)
                      .HasConstraintName("FK_recipes_items_item_id")
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.ingredients)
                      .WithOne(e => e.recipe)
                      .HasForeignKey(e => e.recipe_id)
                      .HasConstraintName("FK_recipe_ingredients_recipes_recipe_id")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RecipeIngredientModel>(entity =>
            {
                entity.ToTable("ingredients");
                entity.HasKey(e => new { e.recipe_id, e.ingredient_id });
                entity.Property(e => e.quantity).IsRequired();
                entity.HasOne(e => e.ingredient)
                      .WithMany()
                      .HasForeignKey(e => e.ingredient_id);
            });

            modelBuilder.Entity<CraftModel>(entity =>
            {
                entity.ToTable("crafts");
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).IsRequired();
                entity.Property(e => e.user_id).IsRequired();
                entity.Property(e => e.recire_id).IsRequired();
                entity.HasOne(e => e.recipe)
                      .WithMany()
                      .HasForeignKey(e => e.recire_id);
                entity.Property(e => e.dt_end).IsRequired();
            });

            HeroSeedData.Seed(modelBuilder);
            ItemsSeedData.Seed(modelBuilder);
            RecipesSeedData.Seed(modelBuilder);
            RecipeIngredientSeedData.Seed(modelBuilder);
        }
    }
}
