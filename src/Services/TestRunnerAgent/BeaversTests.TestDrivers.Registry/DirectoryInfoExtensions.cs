namespace BeaversTests.TestDrivers.Registry;

public static class DirectoryInfoExtensions
{
    public static bool TryDelete(this DirectoryInfo directory, bool recursive, out Exception? exception)
    {
        try
        {
            directory.Delete(recursive);
            exception = null;
            
            return true;
        }
        catch (Exception ex)
        {
            exception = ex;
            
            return false;
        }
    }
}