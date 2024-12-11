namespace BeaversTests.TestsManager.App.Dtos.TestDriver;

public class NewTestDriverDto
{
    public Guid AgId { get; } = Guid.NewGuid();
    public string UserId { get; internal set; } = null!;
    public required string Key { get; init; }
    public required NewTestDriverContentDto Content { get; init; }
    public string? Description { get; init; }
}

public class NewTestDriverContentDto : EntityContentDto { }