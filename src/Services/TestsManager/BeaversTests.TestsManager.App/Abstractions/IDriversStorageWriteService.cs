using BeaversTests.Common.Binary;

namespace BeaversTests.TestsManager.App.Abstractions;

public interface IDriversStorageWriteService
{
    public Task AddTestDriverAsync(
        string testDriverKey, 
        FileSystemEntity testPackageContent,
        CancellationToken cancellationToken = default);

    Task RemoveTestDriverAsync(
        string testDriverKey,
        CancellationToken cancellationToken = default);
}