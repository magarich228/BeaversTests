using BeaversTests.TestsManager.App.Dtos;

namespace BeaversTests.TestsManager.App.Abstractions;

public interface ITestPackageContentExtractor<in TTestPackage>
    where TTestPackage : TestPackageBase

{
    NewTestPackageContentDto ExtractContent(TTestPackage input);
}