using System;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;
using Warranty.Core.Enumerations;
using Warranty.Tests.Core;

namespace Warranty.IntegrationTests.PersistenceTests
{
    public class ActivityLogMapTester : PersistenceTesterBase
    {
        private ActivityLog _activityLog;

        [SetUp]
        public void TestFixtureSetup()
        {
            _activityLog = new ActivityLog
                {
                    ActivityName = "Test",
                    ActivityType = ActivityType.Escalation,
                    ReferenceType = ReferenceType.ServiceCall,
                    Details = "These are the details",
                    ReferenceId = Guid.NewGuid()
                };

            Insert(_activityLog);
        }

        [Test]
        public void ActivityLog_Should_Be_Inserted_With_Values()
        {
            var persistedActivityLog = Load<ActivityLog>(_activityLog.ActivityLogId);
            var userName = new TestWarrantyUserSession().GetCurrentUser().UserName;

            persistedActivityLog.ShouldNotBeNull();
            persistedActivityLog.ActivityName.ShouldEqual(_activityLog.ActivityName);
            persistedActivityLog.ActivityType.ShouldEqual(_activityLog.ActivityType);
            persistedActivityLog.ReferenceType.ShouldEqual(_activityLog.ReferenceType);
            persistedActivityLog.Details.ShouldEqual(_activityLog.Details);
            persistedActivityLog.ReferenceId.ShouldEqual(_activityLog.ReferenceId);
            persistedActivityLog.CreatedDate.ShouldEqual(_activityLog.CreatedDate);
            persistedActivityLog.CreatedBy.ShouldEqual(userName);
        }
    }
}