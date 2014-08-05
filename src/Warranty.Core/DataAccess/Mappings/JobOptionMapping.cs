using NHibernate.Mapping.ByCode.Conformist;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class JobOptionMapping : AuditableEntityMapping<JobOption>
    {
        public JobOptionMapping()
        {
            Table("JobOptions");

            Id(x => x.JobOptionId);
            Property(x => x.JobId);
            Property(x => x.OptionNumber);
            Property(x => x.OptionDescription);
            Property(x => x.Quantity);
        }
    }
}