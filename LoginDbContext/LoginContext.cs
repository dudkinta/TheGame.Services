using LoginDbContext.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginDbContext
{
    public class LoginContext : DbContext, ILoginContext
    {
        public LoginContext(DbContextOptions<LoginContext> options)
            : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }

        public async Task<int> SaveAsync(CancellationToken cancellationToken)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>().ToTable("users");

            modelBuilder.Entity<UserModel>()
            .HasKey(e => e.id);
        }
    }
}
