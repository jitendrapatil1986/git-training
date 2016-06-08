namespace Warranty.Core.Features.Report.WSROpenActivity
{
    public class WSROpenActivityQuery : IQuery<WSROpenActivityModel>
    {
        public WSROpenActivityQuery(WSROpenActivityModel model)
        {
            Model = model;
        }

        public WSROpenActivityQuery() { }

        public WSROpenActivityModel Model { get; set; }
    }
}