using NUnit.Framework;
using Should;
using StructureMap;
using Warranty.Core.Entities;
using Warranty.Core.Services;
using Warranty.Server.Handlers.Jobs;

namespace Warranty.Server.IntegrationTests.Services
{
    [TestFixture]
    public class JobServiceTester : ServiceTesterBase
    {
        private IJobService _jobService;

        public JobServiceTester()
        {
            _jobService = ObjectFactory.GetInstance<IJobService>();
        }

        [Test]
        public void GetJobByNumberTest()
        {
            var insertedJob = Get<Job>();
            var queriedJob = _jobService.GetJobByNumber(insertedJob.JobNumber);

            queriedJob.JobId.ShouldEqual(insertedJob.JobId);
        }
    }
}
