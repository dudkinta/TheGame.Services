using HuntingDbContext.Models;
using Microsoft.EntityFrameworkCore;

namespace HuntingDbContext
{
    public interface IHuntingContext
    {
        DbSet<GameModel> Games { get; set; }
        Task<int> SaveAsync(CancellationToken cancellationToken);
    }
}
