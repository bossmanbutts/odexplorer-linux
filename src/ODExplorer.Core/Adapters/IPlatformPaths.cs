namespace ODExplorer.Adapters
{
    public interface IPlatformPaths
    {
        string GetBaseDirectory();
        string GetResourcePath(string relativePath);
    }

    public class NoOpPlatformPaths : IPlatformPaths
    {
        public string GetBaseDirectory() => System.AppContext.BaseDirectory;
        public string GetResourcePath(string relativePath) => System.IO.Path.Combine(GetBaseDirectory(), relativePath);
    }
}
