namespace BeaversTests.TestsManager.App.Dtos.TestDriver;

public class NewTestDriverDto
{
    public required string Key { get; init; }
    public required NewTestDriverContentDto Content { get; init; }
    public string? Description { get; init; }
}

public class NewTestDriverContentDto : EntityContentDto { }