using BeaversTests.TestsManager.Core.TestPackage;

namespace BeaversTests.TestsManager.Core;

public class BeaversTestsDirectory
{
    public required string DirectoryName { get; init; }
    public required IEnumerable<BeaversTestsFile> TestFiles { get; init; }
    public required IEnumerable<BeaversTestsDirectory> Directories { get; init; }
}