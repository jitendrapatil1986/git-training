using NUnit.Framework;
using Should;
using StructureMap;
using Warranty.Core.Entities;
using Warranty.Server.Handlers.Jobs;
using Warranty.Server.IntegrationTests.SetUp;

namespace Warranty.Server.IntegrationTests.Services
{
    [TestFixture]
    public class HomeOwnerServiceTester : ServiceTesterBase
    {
        private IHomeOwnerService _homeOwnerService;

        public HomeOwnerServiceTester()
        {
            _homeOwnerService = ObjectFactory.GetInstance<IHomeOwnerService>();
        }

        [Test]
        public void Homeowners_by_job_number_query_test()
        {
            var job = Get<Job>();
            var homeOwner = ObjectFactory.GetInstance<EntityBuilder<HomeOwner>>().GetSaved(h => { h.JobId = job.JobId; });
           
            homeOwner.JobId = job.JobId;
            job.CurrentHomeOwnerId = homeOwner.HomeOwnerId;

            using (TestDatabase)
            {
                TestDatabase.Save<Job>(job);
                TestDatabase.Save<HomeOwner>(homeOwner);

                var savedHomeOwner = _homeOwnerService.GetHomeOwnerByJobNumber(job.JobNumber);
                savedHomeOwner.JobId.ShouldEqual(job.JobId);
                savedHomeOwner.HomeOwnerId.ShouldEqual(homeOwner.HomeOwnerId);
                
                job.CurrentHomeOwnerId = null;
                TestDatabase.Update(job);
                TestDatabase.Delete<HomeOwner>(savedHomeOwner);
                TestDatabase.Delete(job);
            }
        }
    }
}