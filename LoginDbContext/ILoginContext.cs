using LoginDbContext.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginDbContext
{
    public interface ILoginContext
    {
        DbSet<UserModel> Users { get; set; }
        Task<int> SaveAsync(CancellationToken cancellationToken);
    }
}
