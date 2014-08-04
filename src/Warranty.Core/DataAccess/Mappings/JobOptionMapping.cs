using NHibernate.Mapping.ByCode.Conformist;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class JobOptionMapping : ClassMapping<JobOption>
    {
        public JobOptionMapping()
        {
            Table("JobOptions");

            Id(x => x.JobOptionId);
            Property(x => x.JobId);
            Property(x => x.OptionNumber);
            Property(x => x.OptionDescription);
            Property(x => x.Quantity);
            Property(x => x.CreatedDate);
            Property(x => x.CreatedBy);
            Property(x => x.UpdatedDate);
            Property(x => x.UpdatedBy);
        }
    }
}