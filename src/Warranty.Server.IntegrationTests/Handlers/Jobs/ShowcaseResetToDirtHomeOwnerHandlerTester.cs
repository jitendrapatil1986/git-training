using NUnit.Framework;
using Should;
using TIPS.Events.JobEvents;
using Warranty.Core.Entities;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class ShowcaseResetToDirtHomeOwnerHandlerTester : HandlerTester<ShowcaseResetToDirt>
    {
        private Job _job;

        [TestFixtureSetUp]
        public void Setup()
        {
            var homeOwner1 = GetSaved<HomeOwner>();
            var homeOwner2 = GetSaved<HomeOwner>(owner =>
            {
                var homeOwnerNumberUniqueKeyViolation = homeOwner1.HomeOwnerNumber == owner.HomeOwnerNumber;

                if (homeOwnerNumberUniqueKeyViolation)
                    owner.HomeOwnerNumber += 1;
            });

            _job = GetSaved<Job>(job =>
            {
                job.CurrentHomeOwnerId = homeOwner1.HomeOwnerId;
            });

            var owner1 = TestDatabase.Single<HomeOwner>("WHERE HomeOwnerId = @0;", homeOwner1.HomeOwnerId);
            owner1.JobId = _job.JobId;
            TestDatabase.Update(owner1);

            var owner2 = TestDatabase.Single<HomeOwner>("WHERE HomeOwnerId = @0;", homeOwner2.HomeOwnerId);
            owner2.JobId = _job.JobId;
            TestDatabase.Update(owner2);

            Send(x =>
            {
                x.JobNumber = _job.JobNumber;
            });
        }

        [Test]
        public void Should_Delete_All_HomeOwners_Assigned_To_Job()
        {
            var homeOwners = TestDatabase.Fetch<HomeOwner>("WHERE JobId = @0;", _job.JobId);
            homeOwners.Count.ShouldEqual(0);
        }
    }
}