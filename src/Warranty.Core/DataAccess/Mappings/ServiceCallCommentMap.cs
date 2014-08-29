namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class ServiceCallCommentMap : AuditableEntityMap<ServiceCallComment>
    {
        public ServiceCallCommentMap()
        {
            TableName("ServiceCallComments")
                .PrimaryKey("ServiceCallCommentId", false)
                .Columns(x =>
                {
                    x.Column(y => y.ServiceCallId);
                    x.Column(y => y.Comment).WithName("ServiceCallComment");
                });
        }
    }
}
