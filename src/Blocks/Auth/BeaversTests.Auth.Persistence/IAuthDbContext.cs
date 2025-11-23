using Microsoft.EntityFrameworkCore;

namespace BeaversTests.Auth.Persistence;

internal interface IAuthDbContext
{
    DbSet<BeaversUser> Users { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}