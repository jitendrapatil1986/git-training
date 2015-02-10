namespace Warranty.Core.Features.Report.WSROpenedClosedCalls
{
    public class WSROpenedClosedCallsQuery : IQuery<WSROpenedClosedCallsModel>
    {
        public WSROpenedClosedCallsModel queryModel { get; set; }

        public WSROpenedClosedCallsQuery()
        {
            queryModel = new WSROpenedClosedCallsModel();
        }
    }
}