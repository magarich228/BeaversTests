namespace BeaversTests.TestsManager.App.Dtos.TestDriver;

public class NewTestDriverDto
{
    public required Guid Id { get; init; }
    public string UserId { get; internal set; }
    public required string Key { get; init; }
    public required NewTestDriverContentDto Content { get; init; }
    public string? Description { get; init; }
}

public class NewTestDriverContentDto : EntityContentDto { }