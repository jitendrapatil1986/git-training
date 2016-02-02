using System;
using NUnit.Framework;
using Should;
using StructureMap;
using TIPS.Events.JobEvents;
using Warranty.Core.Entities;
using Warranty.Server.Extensions;
using Warranty.Server.Handlers.Jobs;
using Job = Warranty.Core.Entities.Job;

namespace Warranty.Server.IntegrationTests.Handlers.Jobs
{
    [TestFixture]
    public class JobSaleCancelledHandlerTester : IBusHandlerTesterBase<JobSaleCancelled>
    {
        private IHomeOwnerService _homeOwnerService;

        public JobSaleCancelledHandlerTester()
        {
            _homeOwnerService = ObjectFactory.GetInstance<IHomeOwnerService>(); ;
        }
        public void SendMessage(string jobNumber, Guid? contactId = null)
        {
            var message = new JobSaleCancelled
            {
                JobNumber = jobNumber
            };
            if (contactId.HasValue)
            {
                message.ContactId = contactId.Value;
            }
            Send(message);
        }

        [Test]
        public void Homeowner_Should_Be_Deleted()
        {
            var job = GetSaved<Job>();
            var homeOwner = GetSaved<HomeOwner>(h => { h.JobId = job.JobId; });

            var homeOwnersFromDb = _homeOwnerService.GetHomeOwnerByJobNumber(job.JobNumber);
            homeOwnersFromDb.ShouldNotBeNull();

            SendMessage(job.JobNumber);

            homeOwnersFromDb = _homeOwnerService.GetHomeOwnerByJobNumber(job.JobNumber);
            homeOwnersFromDb.ShouldBeNull();
        }
    }
}