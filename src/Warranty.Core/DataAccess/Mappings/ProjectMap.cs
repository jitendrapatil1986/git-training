namespace Warranty.Core.DataAccess.Mappings
{
    using Entities;

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