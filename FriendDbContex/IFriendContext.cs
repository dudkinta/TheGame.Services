using FriendDbContex.Models;
using Microsoft.EntityFrameworkCore;

namespace FriendDbContex
{
    public interface IFriendContext
    {
        DbSet<FriendModel> Friends { get; set; }
        Task<int> SaveAsync(CancellationToken cancellationToken);
    }
}
