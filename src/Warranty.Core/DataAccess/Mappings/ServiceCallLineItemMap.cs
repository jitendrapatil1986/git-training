namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class ServiceCallLineItemMap : AuditableEntityMap<ServiceCallLineItem>
    {
        public ServiceCallLineItemMap()
        {
            TableName("ServiceCallLineItems")
                .PrimaryKey("ServiceCallLineItemId", false)
                .Columns(x =>
                {
                    x.Column(y => y.ServiceCallId);
                    x.Column(y => y.LineNumber);
                    x.Column(y => y.ProblemCode);
                    x.Column(y => y.ProblemDescription);
                    x.Column(y => y.CauseDescription);
                    x.Column(y => y.ClassificationNote);
                    x.Column(y => y.LineItemRoot);
                    x.Column(y => y.ServiceCallLineItemStatus).WithName("ServiceCallLineItemStatusId");
                    x.Column(y => y.RootCause);
                    x.Column(y => y.RootProblem);
                    x.Column(y => y.LastCompletedDate);
                });
        }
    }
}