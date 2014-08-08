namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;

    public class HomeOwnerMapping : AuditableEntityMapping<HomeOwner>
    {
        public HomeOwnerMapping()
        {
            Table("HomeOwners");

            Id(x => x.HomeOwnerId, map => map.Generator(Generators.GuidComb));
            Property(x => x.JobId);
            Property(x => x.HomeOwnerNumber);
            Property(x => x.HomeOwnerName);
            Property(x => x.HomePhone);
            Property(x => x.OtherPhone);
            Property(x => x.WorkPhone1);
            Property(x => x.WorkPhone2);
            Property(x => x.EmailAddress);
        }
    }
}