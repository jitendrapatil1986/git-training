using NHibernate.Mapping.ByCode.Conformist;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class CommunityMapping : AuditableEntityMapping<Community>
    {
        public CommunityMapping()
        {
            Table("Communities");

            Id(x => x.CommunityId);
            Property(x => x.CommunityNumber);
            Property(x => x.CommunityName);
            Property(x => x.CityId);
            Property(x => x.DivisionId);
            Property(x => x.ProjectId);
            Property(x => x.SateliteCityId);
            Property(x => x.CommunityStatusCode);
            Property(x => x.CommunityStatusDescription);
            Property(x => x.ProductType);
            Property(x => x.PlanType);
        }
    }
}