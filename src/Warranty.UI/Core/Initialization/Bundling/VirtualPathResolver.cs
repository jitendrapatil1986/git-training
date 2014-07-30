using System.Web;
using System.Web.Hosting;
using dotless.Core.Input;

namespace Warranty.UI.Core.Initialization.Bundling
{
    public class VirtualPathResolver : IPathResolver
    {
        private string _currentFilePath;
        private string _currentFileDirectory;

        public string CurrentFilePath
        {
            get { return _currentFilePath; }
            set
            {
                _currentFilePath = value;
                _currentFileDirectory = VirtualPathUtility.GetDirectory(value);
            }
        }

        public string GetFullPath(string path)
        {
            if (path.StartsWith("~"))
                return path;

            if (VirtualPathUtility.IsAbsolute(path))
                return VirtualPathUtility.ToAppRelative(path,
                    HostingEnvironment.IsHosted ? HostingEnvironment.ApplicationVirtualPath : "/");

            return VirtualPathUtility.Combine(_currentFileDirectory, path);
        }
    }
}