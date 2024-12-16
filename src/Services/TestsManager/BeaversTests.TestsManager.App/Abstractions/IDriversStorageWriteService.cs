using BeaversTests.Common.Binary;

namespace BeaversTests.TestsManager.App.Abstractions;

public interface IDriversStorageWriteService
{
    public Task AddTestDriverAsync(
        string testDriverKey,
        Guid agId,
        FileSystemEntity testPackageContent,
        CancellationToken cancellationToken = default);

    Task RemoveTestDriverAsync(
        string testDriverKey,
        Guid agId,
        CancellationToken cancellationToken = default);
}