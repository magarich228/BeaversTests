using BeaversTests.Common.Application;
using BeaversTests.Common.CQRS;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.Postgres.EventStore;

public class PostgresEventStore(DbContextOptions<PostgresEventStore> options) : DbContext(options), IStore
{
    public DbSet<StreamState> Streams { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new StreamStateConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
    
    public async Task AddAsync(StreamState stream, CancellationToken cancellationToken = default)
    {
        await base.AddAsync(stream, cancellationToken);
        await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<StreamState>> GetEventsAsync(AggregateInfo info, CancellationToken cancellationToken = default)
    {
        var query = this.Streams
            .Where(s => s.AggregateId.Equals(info.Id))
            .WhereIf(info.Version.HasValue, 
                (s) => s.Version.Equals(info.Version))
            .WhereIf(info.CreatedUtc.HasValue, 
                (s) => s.CreatedUtc.Equals(info.CreatedUtc));

        return await query.AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}