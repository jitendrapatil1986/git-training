namespace Warranty.UI.Core.Initialization.Bundling
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web.Hosting;
    using System.Web.Optimization;
    using dotless.Core;
    using dotless.Core.Abstractions;
    using dotless.Core.Importers;
    using dotless.Core.Input;
    using dotless.Core.Loggers;
    using dotless.Core.Parser;

    public class LessBundleTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            context.HttpContext.Response.Cache.SetLastModifiedFromFileDependencies();

            var parser = new Parser();
            var engine = CreateLessEngine(parser);

            var content = new StringBuilder();
            var bundleFiles = new List<VirtualFile>();
            foreach (var bundleFile in response.Files)
            {
                var virtualFile = bundleFile.VirtualFile;
                var virtualPath = virtualFile.VirtualPath;

                bundleFiles.Add(virtualFile);

                SetCurrentFilePath(parser, virtualPath);

                using (var reader = new StreamReader(VirtualPathProvider.OpenFile(virtualPath)))
                {
                    var transformed = engine.TransformToCss(reader.ReadToEnd(), virtualPath);
                    content.Append(transformed);
                    content.AppendLine();

                    bundleFiles.AddRange(GetFileDependencies(parser));
                }
            }

            if (BundleTable.EnableOptimizations)
            {
                response.Files = bundleFiles.Distinct().Select(x => new BundleFile(x.VirtualPath, x));
            }

            response.ContentType = "text/css";
            response.Content = content.ToString();
        }

        private IEnumerable<VirtualFile> GetFileDependencies(Parser parser)
        {
            var pathResolver = GetPathResolver(parser);
            foreach (var path in parser.Importer.Imports)
            {
                yield return HostingEnvironment.VirtualPathProvider.GetFile(pathResolver.GetFullPath(path));
            }
            parser.Importer.Imports.Clear();
        }

        private IPathResolver GetPathResolver(Parser parser)
        {
            var importer = parser.Importer as Importer;
            var reader = importer.FileReader as VirtualFileReader;
            return reader.PathResolver;
        }

        private void SetCurrentFilePath(Parser parser, string currentFilePath)
        {
            var importer = parser.Importer as Importer;
            if (importer == null)
                throw new InvalidOperationException("Unexpected dotless importer type.");

            var fileReader = importer.FileReader as VirtualFileReader;
            if (fileReader == null)
            {
                importer.FileReader = new VirtualFileReader(new VirtualPathResolver { CurrentFilePath = currentFilePath });
            }
            else
            {
                var pathResolver = fileReader.PathResolver as VirtualPathResolver;
                if (pathResolver == null)
                {
                    fileReader.PathResolver = new VirtualPathResolver { CurrentFilePath = currentFilePath };
                }
                else
                {
                    pathResolver.CurrentFilePath = currentFilePath;
                }
            }
        }

        private ILessEngine CreateLessEngine(Parser parser)
        {
            var logger = new AspNetTraceLogger(LogLevel.Debug, new Http());
            return new LessEngine(parser, logger, true, false);
        }
    }
}