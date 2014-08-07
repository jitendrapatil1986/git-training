namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;

    public class JobOptionMapping : AuditableEntityMapping<JobOption>
    {
        public JobOptionMapping()
        {
            Table("JobOptions");

            Id(x => x.JobOptionId, map => map.Generator(new GuidCombGeneratorDef()));
            Property(x => x.JobId);
            Property(x => x.OptionNumber);
            Property(x => x.OptionDescription);
            Property(x => x.Quantity);
        }
    }
}