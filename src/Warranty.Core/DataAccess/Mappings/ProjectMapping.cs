namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;
    using NHibernate.Mapping.ByCode;

    public class ProjectMapping : AuditableEntityMapping<Project>
    {
        public ProjectMapping()
        {
            Table("Projects");

            Id(x => x.ProjectId, map => map.Generator(Generators.GuidComb));
            Property(x => x.ProjectNumber);
            Property(x => x.ProjectName);
        }
    }
}