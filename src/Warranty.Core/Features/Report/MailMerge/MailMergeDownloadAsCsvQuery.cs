namespace Warranty.Core.Features.Report.MailMerge
{
    using System;

    public class MailMergeDownloadAsCsvQuery : IQuery<MailMergeDownloadAsCsvModel>
    {
        public MailMergeReport ReportData { get; set; }
        public DateTime Date { get; set; }
    }
}