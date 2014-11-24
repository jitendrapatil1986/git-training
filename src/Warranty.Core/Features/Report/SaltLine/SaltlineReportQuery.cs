namespace Warranty.Core.Features.Report.Saltline
{
    using System;

    public class SaltlineReportQuery : IQuery<SaltlineReportModel>
    {
        public SaltlineReportModel queryModel { get; set; }

        public SaltlineReportQuery()
        {
            queryModel = new SaltlineReportModel();
        }
    }
}