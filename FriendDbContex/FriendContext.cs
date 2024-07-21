using FriendDbContex.Models;
using Microsoft.EntityFrameworkCore;

namespace FriendDbContex
{
    public class FriendContext : DbContext, IFriendContext
    {
        public FriendContext(DbContextOptions<FriendContext> options)
            : base(options)
        {
        }

        public DbSet<FriendModel> Friends { get; set; }

        public async Task<int> SaveAsync(CancellationToken cancellationToken)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FriendModel>().ToTable("friends");

            modelBuilder.Entity<FriendModel>()
            .HasKey(e => e.id);
        }
    }
}
