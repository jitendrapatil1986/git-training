namespace Warranty.Core.Features.Report.WSRCallSummary
{
    public class WSRCallSummaryQuery : IQuery<WSRCallSummaryModel>
    {
        public WSRCallSummaryModel queryModel { get; set; }

        public WSRCallSummaryQuery()
        {
            queryModel = new WSRCallSummaryModel();
        }
    }
}