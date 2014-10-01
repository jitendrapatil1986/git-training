using System;
using Warranty.Core.Enumerations;

namespace Warranty.Core.ActivityLogger
{
    public interface IActivityLogger
    {
        void Write(string activityName, string details, Guid referenceId, ActivityType activityType, ReferenceType referenceType);
    }
}