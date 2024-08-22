﻿namespace BeaversTests.TestsManager.Core.Models;

public class TestPackageContentDirectory
{
    public required string DirectoryName { get; init; }
    public required IEnumerable<TestPackageFile> TestFiles { get; init; }
    public required IEnumerable<TestPackageContentDirectory> Directories { get; init; }
}