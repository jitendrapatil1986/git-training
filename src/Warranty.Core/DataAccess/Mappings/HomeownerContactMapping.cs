namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;

    public class HomeownerContactMapping : AuditableEntityMapping<HomeownerContact>
    {
        public HomeownerContactMapping()
        {
            Table("HomeownerContacts");

            Id(x => x.HomeownerContactId, map => map.Generator(Generators.GuidComb));
            Property(x => x.HomeownerId);
            Property(x => x.ContactType);
            Property(x => x.ContactValue);
        }
    }
}