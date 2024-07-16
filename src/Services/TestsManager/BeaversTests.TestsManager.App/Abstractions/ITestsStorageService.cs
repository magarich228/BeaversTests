namespace BeaversTests.TestsManager.App.Abstractions;

public interface ITestsStorageService
{
    public Task AddTestAssembly(string name, byte[] testProject, Guid projectId);
}