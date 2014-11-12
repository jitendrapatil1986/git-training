using Warranty.Core.Entities;

namespace Warranty.Core.DataAccess.Mappings
{
    public class ActivityLogMap : AuditCreatedEntityMap<ActivityLog>
    {
        public ActivityLogMap()
        {
            TableName("ActivityLog")
                .PrimaryKey("ActivityLogId", false)
                .Columns(x =>
                {
                    x.Column(y => y.ActivityName);
                    x.Column(y => y.ActivityType);
                    x.Column(y => y.Details);
                    x.Column(y => y.ReferenceId);
                    x.Column(y => y.ReferenceType);
                });
        }
    }
}