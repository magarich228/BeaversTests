using BeaversTests.Common.Binary;
using BeaversTests.TestDrivers.Registry;

namespace BeaversTests.TestRunner;

public class DriverValidationCommand : Command
{
    public string DriverKey { get; init; } = null!;
    public Guid AgId { get; init; }
    public TestDriverContent Driver { get; init; } = null!;
    
    internal override Result Execute()
    {
        using var registry = new TestDriversRegistry(new RegistryConfiguration());

        try
        {
            registry.Register(DriverKey, AgId, Driver);

            _ = registry.GetDriverTestExplorer(DriverKey, AgId, true);
        }
        catch (Exception ex)
        {
            return new Result
            {
                ValidationStatus = "Failure",
                ValidationMessage = ex.ToString()
            };
        }

        return new Result()
        {
            ValidationStatus = "Success"
        };
    }

    public class Result : CommandResult
    {
        public string ValidationStatus { get; set; } = null!;
        public string? ValidationMessage { get; set; }
    }
}