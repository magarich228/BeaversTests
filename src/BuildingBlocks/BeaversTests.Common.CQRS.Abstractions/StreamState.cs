namespace BeaversTests.Common.CQRS.Abstractions;

public class StreamState
{
    public Guid Id { get; } = Guid.NewGuid();
    public Guid AggregateId { get; set; }
    public DateTime CreatedUtc { get; } = DateTime.UtcNow;
    public required string Type { get; set; }
    public required string Data { get; set; }
    public int Version { get; set; } = 0;
}