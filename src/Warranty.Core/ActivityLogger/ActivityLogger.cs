using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;

namespace Warranty.Core.ActivityLogger
{
    public class ActivityLogger : IActivityLogger
    {
        private readonly IDatabase _database;

        public ActivityLogger(IDatabase database)
        {
            _database = database;
        }

        public void Write(string activityName, string details, Guid referenceId, ActivityType activityType, ReferenceType referenceType)
        {
            using (_database)
            {
                var activityLog = new ActivityLog
                    {
                        ActivityName = activityName,
                        ActivityType = activityType,
                        Details = details,
                        ReferenceId = referenceId,
                        ReferenceType = referenceType,
                    };
                _database.Insert(activityLog);
            }
        }
    }
}
