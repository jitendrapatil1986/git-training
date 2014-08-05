using NHibernate.Mapping.ByCode.Conformist;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class ProjectMapping : AuditableEntityMapping<Project>
    {
        public ProjectMapping()
        {
            Table("Projects");

            Id(x => x.ProjectId);
            Property(x => x.ProjectNumber);
            Property(x => x.ProjectName);
        }
    }
}