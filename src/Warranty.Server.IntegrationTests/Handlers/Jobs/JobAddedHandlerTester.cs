using System.Linq;
using Accounting.Events.Job;
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
                var job = TestDatabase.FetchBy<Job>(sql => sql.Where(j => j.JobNumber == Event.Job)).Single();
                job.ShouldNotBeNull();
            }
        }
    }
}