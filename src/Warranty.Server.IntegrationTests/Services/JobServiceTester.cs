using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Should;
using StructureMap;
using Warranty.Core.Entities;
using Warranty.Server.Handlers.Jobs;
using Warranty.Server.IntegrationTests.Extensions.IDatabase;

namespace Warranty.Server.IntegrationTests.Services
{
    [TestFixture]
    public class JobServiceTester : ServicesTestBase
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
