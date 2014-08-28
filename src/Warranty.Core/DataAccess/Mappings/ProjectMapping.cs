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

    public class ProjectMap : AuditableEntityMap<Project>
    {
        public ProjectMap()
        {
            TableName("Projects");
            PrimaryKey(x => x.ProjectId, false);

            Columns(x =>
                        {
                            x.Column(col => col.ProjectName);
                            x.Column(col => col.ProjectNumber);
                        });
        }
    }
}