namespace BeaversTests.Common.CQRS.Abstractions;

public class AggregateInfo
{
    public required Guid Id { get; set; }
    public int? Version { get; set; }
    public DateTime? CreatedUtc { get; set; }
}