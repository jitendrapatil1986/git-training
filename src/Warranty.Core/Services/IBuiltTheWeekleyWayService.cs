namespace Warranty.Core.Services
{
    using System.Collections.Generic;
    using System.IO;

    public interface IBuiltTheWeekleyWayService
    {
        string GetMarketPath(string market);
        IEnumerable<FileInfo> GetFiles(string market);
        string GetDisplayName(string documentName);
    }
}
