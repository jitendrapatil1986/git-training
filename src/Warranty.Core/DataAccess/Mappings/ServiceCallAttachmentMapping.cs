namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;

    public class ServiceCallAttachmentMapping : AuditableEntityMapping<ServiceCallAttachment>
    {
        public ServiceCallAttachmentMapping()
        {
            Table("ServiceCallAttachments");

            Id(x => x.ServiceCallAttachmentId, map => map.Generator(Generators.GuidComb));
            Property(x => x.ServiceCallId);
            Property(x => x.FilePath);
            Property(x => x.DisplayName);
            Property(x => x.IsDeleted);
            Property(x => x.ServiceCallLineItemId);
        }
    }
}