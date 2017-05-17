namespace Warranty.Core.Features.Report.MailMerge
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Web;
    using CsvBuilder;
    using CsvHelper;
    using Enumerations;

    public class MailMergeDownloadAsCsvQueryHandler : IQueryHandler<MailMergeDownloadAsCsvQuery,MailMergeDownloadAsCsvModel>
    {
        private readonly ICsvBuilder _csvBuilder;

        public MailMergeDownloadAsCsvQueryHandler(ICsvBuilder csvBuilder)
        {
            _csvBuilder = csvBuilder;
        }

        public MailMergeDownloadAsCsvModel Handle(MailMergeDownloadAsCsvQuery query)
        {
            var titleLine = string.Format(",,{0}", Month.FromValue(query.Date.Month).Abbreviation);
            var linesBeforeHeader = new List<string> {titleLine};            
            var csvBytes = _csvBuilder.GetCsvBytes(linesBeforeHeader, query.ReportData.Customers);
            var fileName = string.Format("MailMerge_{0}.csv", query.Date.ToString("MM_yyyy"));

            var model = new MailMergeDownloadAsCsvModel
                {
                    Bytes = csvBytes,
                    FileName = fileName,
                    MimeMapping = MimeMapping.GetMimeMapping(fileName)
                };

            return model;
        }
    }
}