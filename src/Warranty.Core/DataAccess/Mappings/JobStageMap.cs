using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class JobStageMap : AuditableEntityMap<JobStage>
    {
        public JobStageMap()
        {
            TableName("JobStages")
                .CompositePrimaryKey(js => js.JobId, js => js.Stage)
                .Columns(x =>
                {
                    x.Column(y => y.CompletionDate);
                    x.Column(y => y.JdeIdentifier);
                });
        }
    }
}