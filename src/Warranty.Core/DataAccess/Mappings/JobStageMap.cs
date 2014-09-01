using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class JobStageMap : AuditableEntityMap<JobStage>
    {
        public JobStageMap()
        {
            TableName("JobStages")
                .CompositePrimaryKey(js => new {js.JobId, js.Stage})
                .Columns(x =>
                {
                    x.Column(y => y.CompletionDate);
                    x.Column(y => y.JdeIdentifier);
                    x.Column(y => y.CreatedDate);
                    x.Column(y => y.CreatedBy);
                    x.Column(y => y.UpdatedDate);
                    x.Column(y => y.UpdatedBy);
                });
        }
    }
}