using NHibernate.Mapping.ByCode.Conformist;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class ProjectMapping : ClassMapping<Project>
    {
        public ProjectMapping()
        {
            Table("Projects");

            Id(x => x.ProjectId);
            Property(x => x.ProjectNumber);
            Property(x => x.ProjectName);
            Property(x => x.CreatedDate);
            Property(x => x.CreatedBy);
            Property(x => x.UpdatedDate);
            Property(x => x.UpdatedBy);
        }
    }
}