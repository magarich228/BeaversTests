namespace BeaversTests.TestDrivers;

public class DriverKey<TKey> where 
    TKey : class, IDriverKey, new()
{
    public string Key => new TKey().Key;
}