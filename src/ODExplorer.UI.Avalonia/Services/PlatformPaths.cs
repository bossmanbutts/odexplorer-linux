using System.IO;
using System.Reflection;
using ODExplorer.Adapters;

namespace ODExplorer.UI.Avalonia.Services
{
    public class PlatformPaths : IPlatformPaths
    {
        public string GetBaseDirectory() => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? System.AppContext.BaseDirectory;
        public string GetResourcePath(string relativePath) => Path.Combine(GetBaseDirectory(), relativePath);
    }
}
