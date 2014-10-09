namespace Warranty.Core.Features.Report.MailMerge
{
    using System;
    using System.Linq;

    public class MailMergeQuery : IQuery<MailMergeQuery>
    {
        public DateTime? Date { get; set; }
        public MailMergeReport Result { get; set; }
        public bool AnyResults { get { return Result != null && Result.Customers.Any(); } }
    }
}
