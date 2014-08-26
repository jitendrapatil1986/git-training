using Accounting.Events.Job;
using NPoco;
using NUnit.Framework;
using Should;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class JobAddedHandlerTester : HandlerTester<JobAdded>
    {
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var community = GetSaved<Community>();

            Send(x =>
            {
                x.Community = community.CommunityNumber;
                x.Job = "12345678";
            });
        }

        [Test]
        public void Job_Should_Be_Added()
        {
            using (TestDatabase)
            {
                var job = TestDatabase.First<Job>("WHERE JobNumber = @0", Event.Job);
                job.ShouldNotBeNull();
            }
        }
    }
}