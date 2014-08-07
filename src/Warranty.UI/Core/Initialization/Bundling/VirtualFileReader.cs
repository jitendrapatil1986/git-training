namespace Warranty.UI.Core.Initialization.Bundling
{
    using System.IO;
    using System.Web.Hosting;
    using dotless.Core.Input;

    public class VirtualFileReader : IFileReader
    {
        public VirtualFileReader(IPathResolver pathResolver)
        {
            PathResolver = pathResolver;
            PathProvider = HostingEnvironment.VirtualPathProvider;
        }

        internal IPathResolver PathResolver { get; set; }

        protected VirtualPathProvider PathProvider { get; private set; }

        public byte[] GetBinaryFileContents(string fileName)
        {
            fileName = PathResolver.GetFullPath(fileName);
            var virtualFile = PathProvider.GetFile(fileName);
            using (var stream = virtualFile.Open())
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
                return buffer;
            }
        }

        public string GetFileContents(string fileName)
        {
            fileName = PathResolver.GetFullPath(fileName);
            var virtualFile = PathProvider.GetFile(fileName);
            using (var reader = new StreamReader(virtualFile.Open()))
            {
                return reader.ReadToEnd();
            }
        }

        public bool DoesFileExist(string fileName)
        {
            return PathProvider.FileExists(PathResolver.GetFullPath(fileName));
        }
    }
}