using System.Threading.Tasks;

namespace BeaversTests.Drivers.Abstractions
{
    public interface ITestListener
    {
        Task SendAsync(TestEvent @event);
    }
}