namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

    public class ServiceCallAttachmentMap: AuditableEntityMap<ServiceCallAttachment>
    {
        public ServiceCallAttachmentMap()
        {
            TableName("ServiceCallAttachments")
                .PrimaryKey("ServiceCallAttachmentId", false)
                .Columns(x =>
                    {
                        x.Column(y => y.ServiceCallId);
                        x.Column(y => y.FilePath);
                        x.Column(y => y.DisplayName);
                        x.Column(y => y.IsDeleted);
                        x.Column(y => y.ServiceCallLineItemId);
                    });
        }
    }
}