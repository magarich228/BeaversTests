namespace BeaversTests.Common.CQRS;

public class AggregateInfo
{
    public required Guid Id { get; set; }
    public int? Version { get; set; }
    public DateTime? CreatedUtc { get; set; }
}