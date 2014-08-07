namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

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