namespace Warranty.Core.Services
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class BuiltTheWeekleyWayService : IBuiltTheWeekleyWayService
    {
        public string GetMarketPath(string market)
        {
            var btwwPath = ConfigurationManager.AppSettings["BTWWSharePath"];
            var btwwPrefix = ConfigurationManager.AppSettings["BTWWPrefix"];

            var pathToQuery = Path.Combine(btwwPath, string.Format("{0}{1}", btwwPrefix, market));
            return pathToQuery;
        }

        public IEnumerable<FileInfo> GetFiles(string market)
        {
            var marketPath = GetMarketPath(market);
            if (Directory.Exists(marketPath))
            {
                var files = Directory.GetFiles(marketPath)
                    .Select(x => new FileInfo(x)).ToList()
                    .Where(x => !x.Attributes.HasFlag(FileAttributes.Hidden));
                return files;
            }

            return new List<FileInfo>();
        }

        public string GetDisplayName(string documentName)
        {
            var name = documentName.Replace(" ", "");
            name = name.Replace(".doc", "");
            name = name.Replace(".docx", "");
            var eventNameWords = Regex.Split(name, "([A-Za-z]+)").Where(s => s != string.Empty).ToArray().ToArray(); 

            return string.Join(" ", eventNameWords);
        }
    }
}
