using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.App.Dtos.TestPackage;

namespace BeaversTests.TestsManager.App.Abstractions;

public interface ITestPackageContentExtractor<in TTestPackage>
    where TTestPackage : TestPackageBase

{
    NewTestPackageContentDto ExtractContent(TTestPackage input);
}