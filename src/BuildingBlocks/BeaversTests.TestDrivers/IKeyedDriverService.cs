namespace BeaversTests.TestDrivers;

public interface IKeyedDriverService<TKey> where 
    TKey : class, IDriverKey, new()
{
    DriverKey<TKey> DriverKey { get; }
}