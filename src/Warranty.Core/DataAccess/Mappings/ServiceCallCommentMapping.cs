namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;

    public class ServiceCallCommentMapping : AuditableEntityMapping<ServiceCallComment>
    {
        public ServiceCallCommentMapping()
        {
            Table("ServiceCallComments");

            Id(x => x.ServiceCallCommentId, map => map.Generator(Generators.GuidComb));
            Property(x => x.ServiceCallId);
            Property(x => x.Comment, map => map.Column("ServiceCallComment"));
        }
    }
}