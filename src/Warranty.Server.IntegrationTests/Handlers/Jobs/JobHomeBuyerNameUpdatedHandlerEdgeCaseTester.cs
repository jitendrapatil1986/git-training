using Accounting.Events.Job;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture, Explicit]
    public class JobHomeBuyerNameUpdatedHandlerEdgeCaseTester : HandlerTester<JobHomeBuyerNameUpdated>
    {
        private HomeOwner _homeBuyer;
        private Job _job;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _job = GetSaved<Job>();
            _homeBuyer = GetSaved<HomeOwner>(ho => ho.JobId = _job.JobId);

            _job.CurrentHomeOwnerId = _homeBuyer.HomeOwnerId;

            using (TestDatabase)
                TestDatabase.Update(_job);

            Send(x =>
            {
                x.JDEId = _job.JdeIdentifier;
                x.BuyerName = "";
                x.HomeBuyerNumber = "00000000000";
            });
        }

        [Test]
        public void Home_Buyer_Should_Be_Updated()
        {
            var job = Get<Job>(_job.JobId);
            var homeBuyer = Get<HomeOwner>(job.CurrentHomeOwnerId);
            homeBuyer.HomeOwnerName.ShouldBeNull();
        }
    }
}