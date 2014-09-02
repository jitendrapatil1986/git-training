using Accounting.Events.Job;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class JobHomeBuyerNameUpdatedHandlerTester : HandlerTester<JobHomeBuyerNameUpdated>
    {
        private HomeOwner _homeOwner;
        private Job _job;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _job = GetSaved<Job>();
            _homeOwner = GetSaved<HomeOwner>(ho => ho.JobId = _job.JobId);

            _job.CurrentHomeOwnerId = _homeOwner.HomeOwnerId;

            using (TestDatabase)
                TestDatabase.Update(_job);

            Send(x =>
            {
                x.JDEId = _job.JdeIdentifier;
                x.BuyerName = "Jo Test";
            });
        }

        [Test]
        public void Job_Close_Date_Should_Be_Updated()
        {
            var job = Get<Job>(_job.JobId);
            var homeOwner = Get<HomeOwner>(job.CurrentHomeOwnerId);
            homeOwner.HomeOwnerName.ShouldEqual(Event.BuyerName);
        }
    }
}