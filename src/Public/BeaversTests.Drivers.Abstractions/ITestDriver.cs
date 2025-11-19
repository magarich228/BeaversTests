using System.Threading.Tasks;

namespace BeaversTests.Drivers.Abstractions
{
    public interface ITestDriver
    {
        void Load(string path);
        TestSuite Explore(); // TODO: nullable?
        Task Run(ITestListener listener);
    }
    
    // TODO: nuget package
}