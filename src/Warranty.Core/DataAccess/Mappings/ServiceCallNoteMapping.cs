namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;

    public class ServiceCallNoteMapping : AuditableEntityMapping<ServiceCallNote>
    {
        public ServiceCallNoteMapping()
        {
            Table("ServiceCallNotes");

            Id(x => x.ServiceCallNoteId, map => map.Generator(Generators.GuidComb));
            Property(x => x.ServiceCallId);
            Property(x => x.Note, map => map.Column("ServiceCallNote"));
        }
    }
}