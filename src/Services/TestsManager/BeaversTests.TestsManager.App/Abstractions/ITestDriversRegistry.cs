namespace BeaversTests.TestsManager.App.Abstractions;

public interface ITestDriversRegistry
{
    Task RegisterTestDriverAsync(string driverName, string driverPath, CancellationToken cancellationToken = default);
}