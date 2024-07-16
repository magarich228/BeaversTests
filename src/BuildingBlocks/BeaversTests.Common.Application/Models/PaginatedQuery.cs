namespace BeaversTests.Common.Application.Models;

public class PaginatedQuery
{
    public int SkipCount { get; init; }
    public int MaxItemsCount { get; init; } = 10;
}

// TODO: Dto сортировки