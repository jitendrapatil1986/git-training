using Accounting.Events.Job;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class JobResetToDirtHandlerTester : HandlerTester<JobResetToDirt>
    {
        private Job _job;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _job = GetSaved<Job>();

            Send(x =>
            {
                x.JDEId = _job.JdeIdentifier;
            });
        }

        [Test]
        public void Job_Should_Be_Reset()
        {
            var job = Get<Job>(_job.JobId);

            job.CloseDate.ShouldBeNull();
            job.CurrentHomeOwnerId.ShouldBeNull();
            job.PlanType.ShouldBeNull();
            job.PlanTypeDescription.ShouldBeNull();
            job.PlanName.ShouldBeNull();
            job.PlanNumber.ShouldBeNull();
            job.Elevation.ShouldBeNull();
            job.Swing.ShouldBeNull();
            job.WarrantyExpirationDate.ShouldBeNull();
            job.BuilderEmployeeId.ShouldBeNull();
            job.SalesConsultantEmployeeId.ShouldBeNull();
        }
    }
}