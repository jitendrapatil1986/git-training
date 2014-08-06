using NHibernate.Mapping.ByCode;
using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class ProjectMapping : AuditableEntityMapping<Project>
    {
        public ProjectMapping()
        {
            Table("Projects");

            Id(x => x.ProjectId, map => map.Generator(new GuidCombGeneratorDef()));
            Property(x => x.ProjectNumber);
            Property(x => x.ProjectName);
        }
    }
}