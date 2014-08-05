using NHibernate.Mapping.ByCode.Conformist;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class HomeOwnerMapping : AuditableEntityMapping<HomeOwner>
    {
        public HomeOwnerMapping()
        {
            Table("HomeOwners");

            Id(x => x.HomeOwnerId);
            Property(x => x.JobId);
            Property(x => x.OwnerNumber);
            Property(x => x.Name);
            Property(x => x.HomePhone);
            Property(x => x.OtherPhone);
            Property(x => x.WorkPhone1);
            Property(x => x.WorkPhone2);
            Property(x => x.EmailAddress);
        }
    }
}