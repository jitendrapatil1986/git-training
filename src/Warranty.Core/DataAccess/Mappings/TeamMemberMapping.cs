using NHibernate.Mapping.ByCode.Conformist;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class TeamMemberMapping : ClassMapping<TeamMember>
    {
        public TeamMemberMapping()
        {
            Table("TeamMembers");

            Id(x => x.TeamMemberId);
            Property(x => x.Number);
            Property(x => x.Name);
            Property(x => x.Fax);
            Property(x => x.CreatedDate);
            Property(x => x.CreatedBy);
            Property(x => x.UpdatedDate);
            Property(x => x.UpdatedBy);
        }
    }
}